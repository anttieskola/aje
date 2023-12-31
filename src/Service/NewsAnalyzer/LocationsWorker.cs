namespace AJE.Service.NewsAnalyzer;

public class LocationsWorker : BackgroundService
{
    private readonly ILogger<LocationsWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public LocationsWorker(
        ILogger<LocationsWorker> logger,
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
                _logger.LogCritical(e, "Error in locations worker");
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
            .Where(x => x.LocationsVersion == AiGetLocationsQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.LocationsVersion < row.LocationsVersion)
            {
                var locations = JsonSerializer.Deserialize<EquatableList<Location>>(row.Locations);
                if (locations == null)
                    _logger.LogCritical("Failed to deserialize locations for article {Id}", row.Id);
                else
                    await _sender.Send(new ArticleUpdateLocationsCommand { Id = current.Id, LocationsVersion = row.LocationsVersion, Locations = locations }, _cancellationToken);
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
            MaxLocationsVersion = AiGetLocationsQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1,
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            _logger.LogInformation("LocationsWorker article source:{}", article.Source);
            var locations = await _sender.Send(new AiGetLocationsQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = await context.Analyses.FindAsync(article.Id);
            if (analysis != null)
            {
                analysis.LocationsVersion = AiGetLocationsQuery.VERSION;
                analysis.Locations = JsonSerializer.Serialize(locations);
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    LocationsVersion = AiGetLocationsQuery.VERSION,
                    Locations = JsonSerializer.Serialize(locations),
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateLocationsCommand { Id = article.Id, LocationsVersion = AiGetLocationsQuery.VERSION, Locations = locations }, _cancellationToken);
        }
    }
}
