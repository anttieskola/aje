﻿namespace AJE.Domain.Entities;

public record Organization
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Simplified content in english
    /// </summary>
    [JsonPropertyName("contentInEnglish")]
    public string ContentInEnglish { get; set; } = string.Empty;
}
