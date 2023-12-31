namespace AJE.Service.NewsAnalyzer;

public class CorporationsWorker : BackgroundService
{
    private readonly ILogger<CorporationsWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public CorporationsWorker(
        ILogger<CorporationsWorker> logger,
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
                _logger.LogCritical(e, "Error in corporations worker");
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
            .Where(x => x.CorporationsVersion == AiGetCorporationsQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.CorporationsVersion < row.CorporationsVersion)
            {
                var corporations = JsonSerializer.Deserialize<EquatableList<Corporation>>(row.Corporations);
                if (corporations == null)
                    _logger.LogCritical("Failed to deserialize corporations for article {Id}", row.Id);
                else
                    await _sender.Send(new ArticleUpdateCorporationsCommand { Id = current.Id, CorporationsVersion = row.CorporationsVersion, Corporations = corporations }, _cancellationToken);
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
            MaxCorporationsVersion = AiGetCorporationsQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1,
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            _logger.LogInformation("CorporationsWorker article source:{}", article.Source);
            var corporations = await _sender.Send(new AiGetCorporationsQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = await context.Analyses.FindAsync(article.Id);
            if (analysis != null)
            {
                analysis.CorporationsVersion = AiGetCorporationsQuery.VERSION;
                analysis.Corporations = JsonSerializer.Serialize(corporations);
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    CorporationsVersion = AiGetCorporationsQuery.VERSION,
                    Corporations = JsonSerializer.Serialize(corporations),
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateCorporationsCommand { Id = article.Id, CorporationsVersion = AiGetCorporationsQuery.VERSION, Corporations = corporations }, _cancellationToken);
        }
    }
}
