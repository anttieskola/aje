namespace AJE.Domain.Entities;

public class EmbeddingRequest
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}

public class EmbeddingResponse
{
    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; set; }
}