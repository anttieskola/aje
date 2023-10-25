namespace AJE.Domain.Events;

public record ArticleEvent
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("type")]
    public required ArticleEventType Type { get; init; }
}
