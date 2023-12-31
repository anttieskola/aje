namespace AJE.Service.NewsAnalyzer;

public class OrganizationsWorker : BackgroundService
{
    private readonly ILogger<OrganizationsWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public OrganizationsWorker(
        ILogger<OrganizationsWorker> logger,
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
                _logger.LogCritical(e, "Error in organizations worker");
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
            .Where(x => x.OrganizationsVersion == AiGetOrganizationsQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.OrganizationsVersion < row.OrganizationsVersion)
            {
                var organizations = JsonSerializer.Deserialize<EquatableList<Organization>>(row.Organizations);
                if (organizations == null)
                    _logger.LogCritical("Failed to deserialize organizations for article {Id}", row.Id);
                else
                    await _sender.Send(new ArticleUpdateOrganizationsCommand { Id = current.Id, OrganizationsVersion = row.OrganizationsVersion, Organizations = organizations }, _cancellationToken);
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
            MaxOrganizationsVersion = AiGetOrganizationsQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1,
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            _logger.LogInformation("OrganizationsWorker article source:{}", article.Source);
            var organizations = await _sender.Send(new AiGetOrganizationsQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = await context.Analyses.FindAsync(article.Id);
            if (analysis != null)
            {
                analysis.OrganizationsVersion = AiGetOrganizationsQuery.VERSION;
                analysis.Organizations = JsonSerializer.Serialize(organizations);
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    OrganizationsVersion = AiGetOrganizationsQuery.VERSION,
                    Organizations = JsonSerializer.Serialize(organizations)
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateOrganizationsCommand { Id = article.Id, OrganizationsVersion = AiGetOrganizationsQuery.VERSION, Organizations = organizations }, _cancellationToken);
        }
    }
}