namespace AJE.Domain.Events;

[JsonDerivedType(typeof(AiChatStartedEvent), "start")]
[JsonDerivedType(typeof(AiChatInteractionEvent), "interaction")]
public record AiChatEvent
{
    [JsonPropertyName("chatId")]
    public required Guid ChatId { get; init; }

    [JsonPropertyName("startTimestamp")]
    public required DateTimeOffset StartTimestamp { get; init; }
}

public record AiChatStartedEvent : AiChatEvent
{
}

public record AiChatInteractionEvent : AiChatEvent
{
    [JsonPropertyName("interactionId")]
    public required Guid InteractionId { get; init; }

    [JsonPropertyName("interactionTimestamp")]
    public required DateTimeOffset InteractionTimestamp { get; init; }

    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("output")]
    public required string Output { get; init; }
}
