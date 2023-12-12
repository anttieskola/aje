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

#pragma warning disable S2094
public record ArticleAddedEvent : ArticleEvent
{
}
#pragma warning restore S2094

public record ArticleUpdatedEvent : ArticleEvent
{
    [JsonPropertyName("contentUpdated")]
    public required bool ContentUpdated { get; init; }
}
