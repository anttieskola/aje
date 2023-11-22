namespace AJE.Domain.Events;

[JsonDerivedType(typeof(AiChatStartedEvent), "start")]
[JsonDerivedType(typeof(AiChatTokenEvent), "token")]
[JsonDerivedType(typeof(AiChatInteractionEvent), "interaction")]
public record AiChatEvent
{
    [JsonPropertyName("isTest")]
    public bool IsTest { get; init; } = false;

    [JsonPropertyName("chatId")]
    public required Guid ChatId { get; init; }

    [JsonPropertyName("eventTimestamp")]
    public required DateTimeOffset EventTimeStamp { get; init; }
}

#pragma warning disable S2094
public record AiChatStartedEvent : AiChatEvent
{
}
#pragma warning restore S2094

public record AiChatTokenEvent : AiChatEvent
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}

public record AiChatInteractionEvent : AiChatEvent
{
    [JsonPropertyName("interactionId")]
    public required Guid InteractionId { get; init; }

    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("output")]
    public required string Output { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("numberOfTokensEvaluated")]
    public required int NumberOfTokensEvaluated { get; init; }

    [JsonPropertyName("numberOfTokensContext")]
    public required int NumberOfTokensContext { get; init; }
}
