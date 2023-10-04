namespace AJE.Domain.Events;

public record ArticlePublishedEvent
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
}
