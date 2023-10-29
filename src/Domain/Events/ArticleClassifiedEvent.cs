namespace AJE.Domain.Events;

public record ArticleClassifiedEvent
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("source")]
    public required string Source { get; init; }

    [JsonPropertyName("polarity")]
    public required Polarity Polarity { get; init; }

    [JsonPropertyName("polarityVersion")]
    public required int PolarityVersion { get; init; }
}
