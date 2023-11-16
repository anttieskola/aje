namespace AJE.Domain.Entities;

[JsonDerivedType(typeof(MarkdownTextElement), "text")]
[JsonDerivedType(typeof(MarkdownHeaderElement), "header")]
public record MarkdownElement
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}

#pragma warning disable S2094
public record MarkdownTextElement : MarkdownElement
{
}
#pragma warning restore S2094

public record MarkdownHeaderElement : MarkdownElement
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
}
