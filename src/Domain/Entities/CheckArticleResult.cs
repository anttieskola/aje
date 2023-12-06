namespace AJE.Domain.Entities;

public record CheckArticleResult
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("isArticle")]
    public required bool IsArticle { get; init; }

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; init; } = string.Empty;
}
