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
