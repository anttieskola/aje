namespace AJE.Domain.Entities;

/// <summary>
/// Request
/// LibreTranslate API: https://libretranslate.com/docs/
/// </summary>
public class TranslateRequest
{
    [JsonPropertyName("q")]
    public required string Text { get; set; }

    [JsonPropertyName("source")]
    public required string SourceLanguage { get; set; }

    [JsonPropertyName("target")]
    public required string TargetLanguage { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; } = "text";

    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Response
/// LibreTranslate API: https://libretranslate.com/docs/
/// </summary>
public class TranslateResponse
{
    [JsonPropertyName("translatedText")]
    public string TranslatedText { get; set; } = string.Empty;
}
