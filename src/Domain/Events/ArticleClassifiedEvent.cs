namespace AJE.Domain.Events;

public record ArticleClassifiedEvent
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("polarity")]
    public required Polarity Polarity { get; init; }
}
