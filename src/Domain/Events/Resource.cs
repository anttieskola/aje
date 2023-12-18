namespace AJE.Domain.Events;

public static class ResourceEventChannels
{
    public const string LlamaAi = "llama-ai";
}

[JsonDerivedType(typeof(ResourceRequestEvent), "request")]
[JsonDerivedType(typeof(ResourceGrantedEvent), "grant")]
[JsonDerivedType(typeof(ResourceReleasedEvent), "release")]
public record ResourceEvent
{
    [JsonPropertyName("isTest")]
    public bool IsTest { get; set; } = false;

    [JsonPropertyName("resourceIdentifier")]
    public required string ResourceIdentifier { get; init; }
}

public record ResourceRequestEvent : ResourceEvent
{
    [JsonPropertyName("requestId")]
    public required Guid RequestId { get; init; }
}

public record ResourceGrantedEvent : ResourceEvent
{
    [JsonPropertyName("requestId")]
    public required Guid RequestId { get; init; }
}

public record ResourceReleasedEvent : ResourceEvent
{
    [JsonPropertyName("requestId")]
    public required Guid RequestId { get; init; }
}
