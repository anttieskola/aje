namespace AJE.Service.NewsFixer.Infra;

[Table("articles")]
public class ArticleRow
{
    [Key]
    [JsonPropertyName("id")]
    [Column("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("tokenCount")]
    [Column("tokencount")]
    public int TokenCount {get; set;}

    [JsonPropertyName("isValid")]
    [Column("isValid")]
    public bool IsValid { get; set; }
}

public class NewsFixerContext : DbContext
{
    public NewsFixerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleRow> Articles { get; set; } = null!;
}
