
namespace AJE.Service.NewsAnalyzer.Infra;

[Table("events")]
public record ArticleClassifiedEventRecord : ArticleClassifiedEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("id")]
    public int Id { get; init; }
}

public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleClassifiedEventRecord> ArticleClassifiedEventRecords { get; set; } = null!;
}

public class EventSaver : IEventSaver
{
    private readonly DbContextOptions<NewsAnalyzerContext> _dbContextOptions;
    public EventSaver(DbContextOptions<NewsAnalyzerContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task SaveAsync(ArticleClassifiedEvent articleClassifiedEvent, CancellationToken cancellationToken)
    {
        using var context = new NewsAnalyzerContext(_dbContextOptions);
        context.ArticleClassifiedEventRecords.Add(new ArticleClassifiedEventRecord
        {
            Source = articleClassifiedEvent.Source,
            Timestamp = articleClassifiedEvent.Timestamp,
            Polarity = articleClassifiedEvent.Polarity,
            PolarityVersion = articleClassifiedEvent.PolarityVersion,
        });
        await context.SaveChangesAsync(cancellationToken);
    }
}