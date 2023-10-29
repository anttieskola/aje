using AJE.Domain.Entities;

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

        var records = await CreateRecords();

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        await context.ArticleClassifiedEventRecords.AddRangeAsync(records, _cancellationToken);
    }

    private async Task<IEnumerable<ArticleClassifiedEventRecord>> CreateRecords()
    {
        var query = new GetArticlesQuery
        {
            Category = Category.NEWS,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);

        var records = new List<ArticleClassifiedEventRecord>();
        foreach (var article in results.Items)
        {
            var dt = new DateTime(article.Modified, DateTimeKind.Utc);
            var dto = new DateTimeOffset(dt);
            var record = new ArticleClassifiedEventRecord
            {
                Timestamp = dto,
                Source = article.Source,
                Polarity = article.Polarity,
                PolarityVersion = article.PolarityVersion
            };
            records.Add(record);
        }
        return records;
    }
}
