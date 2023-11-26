
namespace AJE.Service.NewsAnalyzer.Infra;

[Table("sentimentpolarities")]
public class ArticleSentimentPolarityRow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("serial")]
    [Column("serial")]
    public int Serial { get; init; }

    [JsonPropertyName("source")]
    [Column("source")]
    public required string Source { get; init; }

    [JsonPropertyName("polarity")]
    [Column("polarity")]
    public required Polarity Polarity { get; init; }

    [JsonPropertyName("polarityVersion")]
    [Column("polarityversion")]
    public required int PolarityVersion { get; init; }
}

public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleSentimentPolarityRow> SentimentPolarities { get; set; } = null!;
}
