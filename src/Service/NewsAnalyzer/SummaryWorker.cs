namespace AJE.Service.NewsAnalyzer;

public class SummaryWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public SummaryWorker(
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    public ArticleCategory? NEWS { get; private set; }

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

    // reloading analysis data from db to Redis
    private async Task ReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.Analyses.AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // update article with saved polarity if they are missing
            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.SummaryVersion < row.SummaryVersion)
            {
                await _sender.Send(new ArticleUpdateSummaryCommand { Id = current.Id, SummaryVersion = row.SummaryVersion, Summary = row.Summary }, _cancellationToken);
            }
        }
    }

    // analyze loop
    private async Task LoopAsync()
    {
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = false,
            IsValidForAnalysis = true,
            MaxSummaryVersion = ArticleGetSummaryQuery.CURRENT_SUMMARY_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);

        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            var summary = await _sender.Send(new ArticleGetSummaryQuery { Article = article }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.SummaryVersion = ArticleGetSummaryQuery.CURRENT_SUMMARY_VERSION;
                analysis.Summary = summary;
            }
            else
            {
                context.Analyses.Add(new ArticleAnalysisRow
                {
                    Id = article.Id,
                    SummaryVersion = ArticleGetSummaryQuery.CURRENT_SUMMARY_VERSION,
                    Summary = summary
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateSummaryCommand { Id = article.Id, SummaryVersion = ArticleGetSummaryQuery.CURRENT_SUMMARY_VERSION, Summary = summary }, _cancellationToken);
        }
    }
}
