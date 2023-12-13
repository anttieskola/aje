
namespace AJE.Service.NewsAnalyzer.Infra;

[Table("sentimentpolarities")]
public class ArticleSentimentPolarityRow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("serial")]
    [Column("serial")]
    public int Serial { get; set; }

    [JsonPropertyName("source")]
    [Column("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("polarity")]
    [Column("polarity")]
    public Polarity Polarity { get; set; }

    [JsonPropertyName("polarityVersion")]
    [Column("polarityversion")]
    public int PolarityVersion { get; set; } = 0;
}

[Table("analyses")]
public class ArticleAnalysisRow
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.Empty;

    [JsonPropertyName("summaryVersion")]
    [Column("summaryversion")]
    public int SummaryVersion { get; set; } = 0;

    [JsonPropertyName("summary")]
    [Column("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleSentimentPolarityRow> SentimentPolarities { get; set; } = null!;
    public DbSet<ArticleAnalysisRow> Analyses { get; set; } = null!;
}
