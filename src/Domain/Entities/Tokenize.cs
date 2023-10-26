namespace AJE.Domain.Entities;

public class TokenizeRequest
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}

public class TokenizeResponse
{
    [JsonPropertyName("tokens")]
    public required int[] Tokens { get; set; }
}

public class DeTokenizeRequest
{
    [JsonPropertyName("tokens")]
    public required int[] Tokens { get; set; }
}

public class DeTokenizeResponse
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}