namespace AJE.Domain.Events;

[JsonDerivedType(typeof(ArticleAddedEvent), "added")]
[JsonDerivedType(typeof(ArticleUpdatedEvent), "updated")]
public record ArticleEvent
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }
}

public record ArticleAddedEvent : ArticleEvent
{
    [JsonPropertyName("published")]
    public required bool Published { get; init; }
}

public record ArticleUpdatedEvent : ArticleEvent
{
    [JsonPropertyName("published")]
    public required bool Published { get; init; }

    [JsonPropertyName("contentUpdated")]
    public required bool ContentUpdated { get; init; }
}
