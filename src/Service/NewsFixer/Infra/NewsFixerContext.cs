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
    public int TokenCount { get; set; } = -1;

    [JsonPropertyName("wordCount")]
    [Column("wordcount")]
    public int WordCount { get; set; } = -1;

    [JsonPropertyName("isValid")]
    [Column("isvalid")]
    public bool IsValid { get; set; } = false;
}

public class NewsFixerContext : DbContext
{
    public NewsFixerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<ArticleRow> Articles { get; set; } = null!;
}
