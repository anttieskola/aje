namespace AJE.Domain.Entities;

public record AiChatOptions
{
    [JsonPropertyName("chatId")]
    public required Guid ChatId { get; init; }
}

public record AiChat
{
    [JsonPropertyName("chatId")]
    public required Guid ChatId { get; init; }

    [JsonPropertyName("startTimestamp")]
    public required DateTimeOffset StartTimestamp { get; init; }

    [JsonPropertyName("history")]
    public EquatableList<AiChatInteractionEntry> Interactions { get; init; } = EquatableList<AiChatInteractionEntry>.Empty;
}

public record AiChatInteractionEntry
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
