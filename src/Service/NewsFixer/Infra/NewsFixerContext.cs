namespace AJE.Service.NewsFixer.Infra;

[Table("articles")]
public class ArticleRow
{
    [Key]
    [JsonPropertyName("id")]
    [Column("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("isValid")]
    [Column("isValid")]
    public required bool IsValid { get; init; }

    [JsonPropertyName("reasoning")]
    [Column("reasoning")]
    public string Reasoning { get; init; } = null!;
}

public class NewsFixerContext : DbContext
{
    public NewsFixerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleRow> Articles { get; set; } = null!;
}
