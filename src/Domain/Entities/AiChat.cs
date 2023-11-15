namespace AJE.Domain.Entities;

// so thinking
// - save information of model used
//    - nothing is stopping us to chat with multiple models
//    - ...
// - main purpose to save chat is to enable history in the context

public record AiChatHistoryEntry
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("output")]
    public required string Output { get; init; }
}

public record AiChatOptions
{

}

public record AiChat
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("history")]
    public EquatableList<AiChatHistoryEntry> History { get; init; } = EquatableList<AiChatHistoryEntry>.Empty;
}
