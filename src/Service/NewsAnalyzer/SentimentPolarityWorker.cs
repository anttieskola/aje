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

    public Category? NEWS { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        // reload data
        await ReloadAsync();

        // analyze/update articles
        while (!_cancellationToken.IsCancellationRequested)
        {
            await LoopAsync();
            // small throttle to give other components time with AI
            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationToken);
        }
    }

    /// <summary>
    /// Utility function to fix article id's because earlier all articles got random id
    /// and now we create id's based on source so they will be identical when source is same
    /// Not used atm, but kept for a while in code just in case.
    /// TODO: remove this after it been in source control for atleast 1 commit
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task FixGuids()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.SentimentPolarities.ToList();
        foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var article = await _sender.Send(new GetArticleQuery { Source = row.Source }, _cancellationToken) ?? throw new Exception($"Article not found for source {row.Source}");
            if (article.Id == row.Id)
                continue;

            var rowWithCorrectId = new ArticleSentimentPolarityRow
            {
                Id = article.Id,
                Timestamp = row.Timestamp,
                Source = row.Source,
                Polarity = row.Polarity,
                PolarityVersion = row.PolarityVersion
            };
            context.SentimentPolarities.Remove(row);
            context.SentimentPolarities.Add(rowWithCorrectId);
            await context.SaveChangesAsync(_cancellationToken);
        }
    }

    // reloading analysis data from db to Redis
    private async Task ReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.SentimentPolarities
            .GroupBy(row => row.Source)
            .Select(group => group.OrderByDescending(row => row.Timestamp).First())
            .AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // update article with saved polarity if they are missing
            var current = await _sender.Send(new GetArticleQuery { Id = row.Id }, _cancellationToken);
            if (current != null)
            {
                if (current.PolarityVersion < row.PolarityVersion)
                {
                    current.Polarity = row.Polarity;
                    current.PolarityVersion = row.PolarityVersion;
                    await _sender.Send(new UpdateArticleCommand { Article = current }, _cancellationToken);
                }
            }
        }
    }

    // Analyze loop, simple way that just analyzes latest article
    // listening to events would be better but this works for now
    private async Task<bool> LoopAsync()
    {
        // get latest article that has not been analyzed
        // or has been analyzed with older polarity version
        var query = new GetArticlesQuery
        {
            Category = Category.NEWS,
            MaxPolarityVersion = GetArticleSentimentPolarityQuery.CURRENT_POLARITY_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            // analyze article "sentiment" polarity
            var article = results.Items.First();
            var command = new GetArticleSentimentPolarityQuery { Article = article };
            var polarityEvent = await _sender.Send(command, _cancellationToken);

            // save result to database
            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
            context.SentimentPolarities.Add(new ArticleSentimentPolarityRow
            {
                Id = article.Id,
                Timestamp = DateTimeOffset.UtcNow,
                Source = article.Source,
                Polarity = polarityEvent.Polarity,
                PolarityVersion = polarityEvent.PolarityVersion
            });
            await context.SaveChangesAsync(_cancellationToken);

            // update article with new polarity
            article.Polarity = polarityEvent.Polarity;
            article.PolarityVersion = polarityEvent.PolarityVersion;
            var updateCommand = new UpdateArticleCommand { Article = article };
            await _sender.Send(updateCommand, _cancellationToken);
            _logger.LogInformation("Updated Article {} with polarity {}", article.Source, article.Polarity);
        }
        return results.TotalCount > 1;
    }
}
