namespace AJE.Domain.Entities;

[JsonDerivedType(typeof(MarkdownTextElement), "text")]
[JsonDerivedType(typeof(MarkdownHeaderElement), "header")]
public record MarkdownElement
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}

public record MarkdownTextElement : MarkdownElement
{
}

public record MarkdownHeaderElement : MarkdownElement
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
}
