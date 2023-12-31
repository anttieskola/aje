namespace AJE.Service.NewsAnalyzer;

public class PositiveThingsWorker : BackgroundService
{
    private readonly ILogger<PositiveThingsWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public PositiveThingsWorker(
        ILogger<PositiveThingsWorker> logger,
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
                _logger.LogCritical(e, "Error in positive things worker");
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
            .Where(x => x.PositiveThingsVersion == AiGetPositiveThingsQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.PositiveThingsVersion < row.PositiveThingsVersion)
            {
                var positiveThings = JsonSerializer.Deserialize<EquatableList<PositiveThing>>(row.PositiveThings);
                if (positiveThings == null)
                    _logger.LogCritical("Failed to deserialize positive things for article {Id}", row.Id);
                else
                    await _sender.Send(new ArticleUpdatePositiveThingsCommand { Id = current.Id, PositiveThingsVersion = row.PositiveThingsVersion, PositiveThings = positiveThings }, _cancellationToken);
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
            MaxPositiveThingsVersion = AiGetPositiveThingsQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);

        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            _logger.LogInformation("PositiveThingsWorker article source:{}", article.Source);
            var positiveThings = await _sender.Send(new AiGetPositiveThingsQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.PositiveThingsVersion = AiGetPositiveThingsQuery.VERSION;
                analysis.PositiveThings = JsonSerializer.Serialize(positiveThings);
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    PositiveThingsVersion = AiGetPositiveThingsQuery.VERSION,
                    PositiveThings = JsonSerializer.Serialize(positiveThings),
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdatePositiveThingsCommand { Id = article.Id, PositiveThingsVersion = AiGetPositiveThingsQuery.VERSION, PositiveThings = positiveThings }, _cancellationToken);
        }
    }
}
