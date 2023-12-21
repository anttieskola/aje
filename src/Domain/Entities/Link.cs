namespace AJE.Domain.Entities;

public record Link
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("uri")]
    public required Uri Uri { get; set; }

    /// <summary>
    /// Simplified content in english
    /// </summary>
    [JsonPropertyName("contentInEnglish")]
    public string ContentInEnglish { get; set; } = string.Empty;
}
