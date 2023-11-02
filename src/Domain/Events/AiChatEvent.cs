namespace AJE.Domain.Events;

// How to define event structure if we want to simulate
// multiple entities?

[JsonDerivedType(typeof(AiChatStartedEvent), "started")]
[JsonDerivedType(typeof(AiChatMessageEvent), "message")]
public record AiChatEvent
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }
}

public record AiChatStartedEvent : AiChatEvent
{
}

public record AiChatMessageEvent : AiChatEvent
{
    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("output")]
    public required string Output { get; init; }
}
