namespace AJE.Domain.Entities;

public record PromptStudioOptions
{
    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; init; }
}

public record PromptStudioSessionHeader
{
    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; set; }

    [JsonPropertyName("title")]
    public required string Title {get; init; }

    [JsonPropertyName("modified")]
    public long Modified { get; init; }
}

public record PromptStudioSession
{
    [JsonPropertyName("sessionId")]
    public required Guid SessionId { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("modified")]
    public long Modified { get; init; } = DateTimeOffset.UtcNow.Ticks;

    [JsonPropertyName("entityName")]
    public string EntityName { get; init; } = string.Empty;

    [JsonPropertyName("systemInstructions")]
    public EquatableList<string> SystemInstructions { get; init; } = [];

    [JsonPropertyName("context")]
    public string Context { get; init; } = string.Empty;

    [JsonPropertyName("runs")]
    public EquatableList<PromptStudioRun> Runs { get; set; } = [];
}

public record PromptStudioRun
{
    [JsonPropertyName("runId")]
    public required Guid RunId { get; init; }

    [JsonPropertyName("entityName")]
    public required string EntityName { get; init; } = string.Empty;

    [JsonPropertyName("systemInstructions")]
    public required EquatableList<string> SystemInstructions { get; init; }

    [JsonPropertyName("context")]
    public required string Context { get; init; } = string.Empty;

    [JsonPropertyName("result")]
    public required string Result { get; init; } = string.Empty;

    [JsonPropertyName("model")]
    public required string Model { get; init; } = string.Empty;

    [JsonPropertyName("numberOfTokensEvaluated")]
    public required int NumberOfTokensEvaluated { get; init; }

    [JsonPropertyName("numberOfTokensContext")]
    public required int NumberOfTokensContext { get; init; }
}