namespace AJE.Domain.Events;

[JsonDerivedType(typeof(PromptStudioStartEvent), "start")]
[JsonDerivedType(typeof(PromptStudioRunTokenEvent), "token")]
[JsonDerivedType(typeof(PromptStudioRunCompletedEvent), "run")]
[JsonDerivedType(typeof(PromptStudioTitleUpdatedEvent), "titleUpdate")]
[JsonDerivedType(typeof(PromptStudioTemperatureUpdatedEvent), "temperatureUpdate")]
[JsonDerivedType(typeof(PromptStudioNumberOfTokensToPredictUpdatedEvent), "numberOfTokensToPredictUpdate")]
[JsonDerivedType(typeof(PromptStudioEntityNameUpdatedEvent), "entityNameUpdate")]
[JsonDerivedType(typeof(PropmtStudioSystemInstructionsUpdatedEvent), "systemInstructionsUpdate")]
[JsonDerivedType(typeof(PromptStudioContextUpdatedEvent), "contextUpdate")]
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

public record PromptStudioTitleUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }
}

public record PromptStudioTemperatureUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("temperature")]
    public required double Temperature { get; init; }
}

public record PromptStudioNumberOfTokensToPredictUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("numberOfTokensToPredict")]
    public required int NumberOfTokensToPredict { get; init; }
}

public record PromptStudioEntityNameUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("entityName")]
    public required string EntityName { get; init; }
}

public record PropmtStudioSystemInstructionsUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("systemInstructions")]
    public required EquatableList<string> SystemInstructions { get; init; }
}

public record PromptStudioContextUpdatedEvent : PromptStudioEvent
{
    [JsonPropertyName("context")]
    public required string Context { get; init; }
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
