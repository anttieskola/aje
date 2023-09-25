namespace AJE.Domain.Entities;

public static class ArticleConstants
{
    public static readonly string IndexName = $"idxArticle";
    public static readonly string IndexPrefix = $"article:";
}

public class ArticleHeader
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

public class ArticleFooter : ArticleHeader
{
    [JsonPropertyName("chat")]
    public List<ChatMessage> Chat { get; set; } = new List<ChatMessage>();
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
    [JsonPropertyName("updated")]
    public DateTime Updated { get; set; }
}

public class Article : ArticleFooter
{
    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
