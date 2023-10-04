namespace AJE.Domain.Entities;

public class NewsArticle
{
    [JsonPropertyName("source")]
    public required string Source { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [JsonPropertyName("content")]
    public IEnumerable<MarkdownElement> Content { get; set; } = Array.Empty<MarkdownElement>();
}
