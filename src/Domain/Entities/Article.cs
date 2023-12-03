namespace AJE.Domain.Entities;

public record ArticleHeader
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("polarity")]
    public Polarity Polarity { get; set; } = Polarity.Unknown;
}

public record Article : ArticleHeader
{
    [JsonPropertyName("category")]
    public ArticleCategory Category { get; set; }

    [JsonPropertyName("modified")]
    public long Modified { get; set; }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("polarityVersion")]
    public int PolarityVersion { get; set; } = 0;

    [JsonPropertyName("content")]
    public EquatableList<MarkdownElement> Content { get; set; } = EquatableList<MarkdownElement>.Empty;

    [JsonPropertyName("chat")]
    public EquatableList<ChatMessage> Chat { get; set; } = EquatableList<ChatMessage>.Empty;
}
