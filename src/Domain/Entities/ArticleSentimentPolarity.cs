namespace AJE.Domain.Entities;

public record ArticleSentimentPolarity
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("source")]
    public required string Source { get; init; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("polarity")]
    public required Polarity Polarity { get; init; }

    [JsonPropertyName("polarityVersion")]
    public required int PolarityVersion { get; init; }
}

