namespace AJE.Domain.Entities;

public record KeyPerson
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("intention")]
    public required string Intention { get; set; }
}
