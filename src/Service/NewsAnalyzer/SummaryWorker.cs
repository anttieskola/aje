namespace AJE.Service.NewsAnalyzer;

public class SummaryWorker : BackgroundService
{
    private readonly ILogger<SummaryWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public SummaryWorker(
        ILogger<SummaryWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
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
            try
            {
                await LoopAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error in summary worker");
                await Task.Delay(TimeSpan.FromMinutes(3), _cancellationToken);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(100), _cancellationToken);
        }
    }

    private async Task ReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.Analyses
            .Where(x => x.SummaryVersion == AiGetSummaryQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.SummaryVersion < row.SummaryVersion)
            {
                await _sender.Send(new ArticleUpdateSummaryCommand { Id = current.Id, SummaryVersion = row.SummaryVersion, Summary = row.Summary }, _cancellationToken);
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
            MaxSummaryVersion = AiGetSummaryQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            _logger.LogInformation("SummaryWorker article source:{}", article.Source);
            var summary = await _sender.Send(new AiGetSummaryQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.SummaryVersion = AiGetSummaryQuery.VERSION;
                analysis.Summary = summary;
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    SummaryVersion = AiGetSummaryQuery.VERSION,
                    Summary = summary
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateSummaryCommand { Id = article.Id, SummaryVersion = AiGetSummaryQuery.VERSION, Summary = summary }, _cancellationToken);
        }
    }
}
