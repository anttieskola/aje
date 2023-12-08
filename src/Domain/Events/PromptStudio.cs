namespace AJE.Domain.Events;

[JsonDerivedType(typeof(PromptStudioStartEvent), "start")]
[JsonDerivedType(typeof(PromptStudioRunTokenEvent), "token")]
[JsonDerivedType(typeof(PromptStudioRunCompletedEvent), "run")]
public record PromptStudioEvent
{
    [JsonPropertyName("isTest")]
    public bool IsTest { get; init; } = false;

    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; init; }

    [JsonPropertyName("eventTimestamp")]
    public required DateTimeOffset EventTimeStamp { get; init; }
}

#pragma warning disable S2094
public record PromptStudioStartEvent : PromptStudioEvent
{
}
#pragma warning restore S2094

public record PromptStudioRunTokenEvent : PromptStudioEvent
{
    [JsonPropertyName("runId")]
    public required Guid RunId { get; init; }

    [JsonPropertyName("token")]
    public required string Token { get; init; }
}

public record PromptStudioRunCompletedEvent : PromptStudioEvent
{
    [JsonPropertyName("runId")]
    public required Guid RunId { get; init; }

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
