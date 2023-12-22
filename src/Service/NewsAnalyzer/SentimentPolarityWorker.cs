namespace AJE.Service.NewsAnalyzer;

public class SentimentPolarityWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public SentimentPolarityWorker(
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;

        await ReloadAsync();

        while (!_cancellationToken.IsCancellationRequested)
        {
            await LoopAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(100), _cancellationToken);
        }
    }

    private async Task ReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.Analyses
            .Where(x => x.PolarityVersion == AiGetSentimentPolarityQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.PolarityVersion < row.PolarityVersion)
            {
                await _sender.Send(new ArticleUpdatePolarityCommand { Id = current.Id, PolarityVersion = row.PolarityVersion, Polarity = row.Polarity }, _cancellationToken);
            }
        }
    }

    private async Task LoopAsync()
    {
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = false,
            IsValidForAnalysis = true,
            MaxPolarityVersion = AiGetSentimentPolarityQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            var polarity = await _sender.Send(new AiGetSentimentPolarityQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.PolarityVersion = AiGetSentimentPolarityQuery.VERSION;
                analysis.Polarity = polarity;
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    PolarityVersion = AiGetSentimentPolarityQuery.VERSION,
                    Polarity = polarity,
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdatePolarityCommand { Id = article.Id, PolarityVersion = AiGetSentimentPolarityQuery.VERSION, Polarity = polarity }, _cancellationToken);
        }
    }
}
