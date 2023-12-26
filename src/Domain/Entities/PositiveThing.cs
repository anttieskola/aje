namespace AJE.Domain.Entities;

public record PositiveThing
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }
}
