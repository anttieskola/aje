namespace AJE.Domain.Entities;

public class AiSettings
{
    /// <summary>
    /// Adjust the randomness of the generated text
    /// (default: 0.8)
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.8;

    /// <summary>
    /// Limit the next token selection to the K most probable tokens
    /// (default: 40)
    /// </summary>
    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 40;

    /// <summary>
    ///  Set the maximum number of tokens to predict when generating text.
    ///  Note: May exceed the set limit slightly if the last token is a partial multibyte character.
    ///  When 0, no tokens will be generated but the prompt is evaluated into the cache.
    ///  (default: -1, -1 = infinity)
    /// </summary>
    [JsonPropertyName("n_predict")]
    public int NumberOfTokensToPredict { get; set; } = -1;
}
