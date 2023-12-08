namespace AJE.Domain.Entities;

public record PromptStudioOptions
{
    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; init; }
}

public record PromptStudioSession
{
    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; set; }

    [JsonPropertyName("runs")]
    public EquatableList<PromptStudioSessionRun> Runs { get; set; } = [];
}

public record PromptStudioSessionRun
{
    [JsonPropertyName("runId")]
    public required Guid RunId { get; init; }

    [JsonPropertyName("entityName")]
    public required string EntityName { get; set; } = string.Empty;

    [JsonPropertyName("systemInstructions")]
    public required EquatableList<string> SystemInstructions { get; set; } = [];

    [JsonPropertyName("context")]
    public required string Context { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public required string Result { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public required string Model { get; init; } = string.Empty;

    [JsonPropertyName("numberOfTokensEvaluated")]
    public required int NumberOfTokensEvaluated { get; init; }

    [JsonPropertyName("numberOfTokensContext")]
    public required int NumberOfTokensContext { get; init; }
}