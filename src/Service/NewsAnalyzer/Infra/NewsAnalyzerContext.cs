
namespace AJE.Service.NewsAnalyzer.Infra;

[Table("sentimentpolarities")]
public record ArticleSentimentPolarityRow : ArticleSentimentPolarity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonPropertyName("serial")]
    public int Serial { get; init; }
}

public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleSentimentPolarityRow> SentimentPolarities { get; set; } = null!;
}
