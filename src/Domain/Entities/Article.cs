namespace AJE.Domain.Entities;

public class ArticleHeader
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

public class Article : ArticleHeader
{
    [JsonPropertyName("category")]
    public Category Category { get; set; }

    [JsonPropertyName("modified")]
    public long Modified { get; set; }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("polarity")]
    public Polarity Polarity { get; set; } = Polarity.Unknown;

    [JsonPropertyName("polarityVersion")]
    public int PolarityVersion { get; set; } = 0;

    [JsonPropertyName("content")]
    public IEnumerable<MarkdownElement> Content { get; set; } = Array.Empty<MarkdownElement>();

    [JsonPropertyName("chat")]
    public IEnumerable<ChatMessage> Chat { get; set; } = Array.Empty<ChatMessage>();
}
