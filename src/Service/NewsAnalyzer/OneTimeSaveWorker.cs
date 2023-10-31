namespace AJE.Service.NewsAnalyzer;

// Testing out database usage and storing already classified article polarities
// This is not permanent solution just temporary thing to save the data currently
// stored in Redis
public class OneTimeSaveWorker : BackgroundService
{
    private readonly ILogger<OneTimeSaveWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;
    public OneTimeSaveWorker(
        ILogger<OneTimeSaveWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var rows = await CreateRows();
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        await context.SentimentPolarities.AddRangeAsync(rows, _cancellationToken);
        await context.SaveChangesAsync(_cancellationToken);
    }

    private async Task<IEnumerable<ArticleSentimentPolarityRow>> CreateRows()
    {
        var query = new GetArticlesQuery
        {
            Category = Category.NEWS,
            Offset = 0,
            PageSize = 10000
        };
        var results = await _sender.Send(query, _cancellationToken);

        var rows = new List<ArticleSentimentPolarityRow>();
        foreach (var article in results.Items)
        {
            var dt = new DateTime(article.Modified, DateTimeKind.Utc);
            var dto = new DateTimeOffset(dt);
            var record = new ArticleSentimentPolarityRow
            {
                Id = article.Id,
                Timestamp = dto,
                Source = article.Source,
                Polarity = article.Polarity,
                PolarityVersion = article.PolarityVersion
            };
            rows.Add(record);
        }
        return rows;
    }
}
