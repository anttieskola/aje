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

    [JsonPropertyName("interactions")]
    public EquatableList<AiChatInteractionEntry> Interactions { get; init; } = [];
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

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("numberOfTokensEvaluated")]
    public required int NumberOfTokensEvaluated { get; init; }

    [JsonPropertyName("numberOfTokensContext")]
    public required int NumberOfTokensContext { get; init; }

}
