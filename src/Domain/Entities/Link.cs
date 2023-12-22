namespace AJE.Domain.Entities;

public record Link
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("uri")]
    public Uri Uri { get; set; } = null!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Simplified content in english
    /// </summary>
    [JsonPropertyName("contentInEnglish")]
    public string ContentInEnglish { get; set; } = string.Empty;
}
