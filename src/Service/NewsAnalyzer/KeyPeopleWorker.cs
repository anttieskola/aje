namespace AJE.Service.NewsAnalyzer;

public class KeyPeopleWorker : BackgroundService
{
    private readonly ILogger<KeyPeopleWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public KeyPeopleWorker(
        ILogger<KeyPeopleWorker> logger,
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
                _logger.LogCritical(e, "Error in key people worker");
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
            .Where(x => x.KeyPeopleVersion == AiGetKeyPeopleQuery.VERSION)
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.KeyPeopleVersion < row.KeyPeopleVersion)
            {
                var keyPeople = JsonSerializer.Deserialize<EquatableList<KeyPerson>>(row.KeyPeople);
                if (keyPeople == null)
                    _logger.LogCritical("Failed to deserialize positive things for article {Id}", row.Id);
                else
                    await _sender.Send(new ArticleUpdateKeyPeopleCommand { Id = current.Id, KeyPeopleVersion = row.KeyPeopleVersion, KeyPeople = keyPeople }, _cancellationToken);
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
            MaxKeyPeopleVersion = AiGetKeyPeopleQuery.VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);

        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            var keyPeople = await _sender.Send(new AiGetKeyPeopleQuery { Context = article.GetContextForAnalysis() }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.KeyPeopleVersion = AiGetKeyPeopleQuery.VERSION;
                analysis.KeyPeople = JsonSerializer.Serialize(keyPeople);
            }
            else
            {
                context.Analyses.Add(new AnalysisRow
                {
                    Id = article.Id,
                    KeyPeopleVersion = AiGetKeyPeopleQuery.VERSION,
                    KeyPeople = JsonSerializer.Serialize(keyPeople),
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdateKeyPeopleCommand { Id = article.Id, KeyPeopleVersion = AiGetKeyPeopleQuery.VERSION, KeyPeople = keyPeople }, _cancellationToken);
        }
    }
}
