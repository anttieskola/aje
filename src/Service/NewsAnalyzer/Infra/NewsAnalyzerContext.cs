
namespace AJE.Service.NewsAnalyzer.Infra;

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

    public NewsAnalyzerContext()
        : base()
    {
    }

    public DbSet<ArticleClassifiedEventRecord> ArticleClassifiedEventRecords { get; set; } = null!;
}

public class EventSaver : IEventSaver
{
    public async Task SaveAsync(ArticleClassifiedEvent articleClassifiedEvent, CancellationToken cancellationToken)
    {
        using var context = new NewsAnalyzerContext();
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