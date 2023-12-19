namespace AJE.Service.NewsAnalyzer;

public class SentimentPolarityWorker : BackgroundService
{
    private readonly ILogger<SentimentPolarityWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public SentimentPolarityWorker(
        ILogger<SentimentPolarityWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    public ArticleCategory? NEWS { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;

        // reload data
        await ReloadAsync();

        // analyze/update articles
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
        var rows = context.SentimentPolarities
            .GroupBy(row => row.Source)
            .Select(group => group.OrderByDescending(row => row.PolarityVersion).First())
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // update article with saved polarity if they are missing
            var current = await _sender.Send(new ArticleGetBySourceQuery { Source = row.Source }, _cancellationToken);
            if (current != null && current.PolarityVersion < row.PolarityVersion)
            {
                await _sender.Send(new ArticleUpdatePolarityCommand { Id = current.Id, PolarityVersion = row.PolarityVersion, Polarity = row.Polarity }, _cancellationToken);
            }
        }
    }

    // Analyze loop, simple way that just analyzes latest article
    // listening to events would be better but this works for now
    private async Task<bool> LoopAsync()
    {
        // get latest article that has not been analyzed
        // or has been analyzed with older polarity version
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = false,
            MaxPolarityVersion = ArticleGetSentimentPolarityQuery.CURRENT_POLARITY_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            // analyze article "sentiment" polarity
            var article = results.Items.First();
            var command = new ArticleGetSentimentPolarityQuery { Article = article };
            var polarityEvent = await _sender.Send(command, _cancellationToken);

            // save result to database
            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
            context.SentimentPolarities.Add(new ArticleSentimentPolarityRow
            {
                Source = article.Source,
                Polarity = polarityEvent.Polarity,
                PolarityVersion = polarityEvent.PolarityVersion
            });
            await context.SaveChangesAsync(_cancellationToken);

            // update article with new polarity
            article.Polarity = polarityEvent.Polarity;
            article.PolarityVersion = polarityEvent.PolarityVersion;
            var updateCommand = new ArticleUpdateCommand { Article = article };
            await _sender.Send(updateCommand, _cancellationToken);
            _logger.LogInformation("Updated Article {} with polarity {}", article.Source, article.Polarity);
        }
        return results.TotalCount > 1;
    }
}
