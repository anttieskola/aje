namespace AJE.Service.NewsTrends;

public class TrendWorker : BackgroundService
{
    private readonly ILogger<TrendWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public TrendWorker(
        ILogger<TrendWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    private ConcurrentBag<NewsPolarityTrendItem> _items = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;

        // analyze/update articles
        while (!_cancellationToken.IsCancellationRequested)
        {
            if (await ReadAllNewsArticles())
            {
            }
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsTrendsContext>();
        }
    }

    private async Task<bool> ReadAllNewsArticles()
    {
        var offset = 0;
        while (!_cancellationToken.IsCancellationRequested)
        {
            var query = new GetArticlesQuery
            {
                Category = ArticleCategory.NEWS,
                MaxPolarityVersion = GetArticleSentimentPolarityQuery.CURRENT_POLARITY_VERSION,
                Offset = offset,
                PageSize = 1000
            };
            var results = await _sender.Send(query, _cancellationToken);
            foreach (var a in results.Items)
            {
                _items.Add(
                    new NewsPolarityTrendItem
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Published = new DateTimeOffset(new DateTime(a.Modified)),
                        Polarity = a.Polarity,
                        PolarityVersion = a.PolarityVersion
                    });
            }
            offset += results.Items.Count;
            if (results.Items.Count == 0)
                break;
        }
        return true;
    }
}
