namespace AJE.Domain.Entities;

public record ArticleHeader
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Sentiment polarity analysis (AI)
    /// </summary>
    [JsonPropertyName("polarity")]
    public Polarity Polarity { get; set; } = Polarity.Unknown;
}

public record Article : ArticleHeader
{
    [JsonPropertyName("category")]
    public ArticleCategory Category { get; set; }

    [JsonPropertyName("content")]
    public EquatableList<MarkdownElement> Content { get; set; } = [];

    [JsonPropertyName("chat")]
    public EquatableList<ChatMessage> Chat { get; set; } = [];

    /// <summary>
    /// Is article live news that updates until some point in time
    /// Will stay true until reporting ended
    /// </summary>
    [JsonPropertyName("isLiveNews")]
    public bool IsLiveNews { get; set; }

    [JsonPropertyName("modified")]
    public long Modified { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Sentiment polarity analysis version (AI)
    /// </summary>
    [JsonPropertyName("polarityVersion")]
    public int PolarityVersion { get; set; } = 0;

    /// <summary>
    /// Is valid article for analysis
    /// </summary>
    [JsonPropertyName("isValidForAnalysis")]
    public bool IsValidForAnalysis { get; set; } = false;

    /// <summary>
    /// Title in english
    /// </summary>
    [JsonPropertyName("titleInEnglish")]
    public string TitleInEnglish { get; set; } = string.Empty;

    /// <summary>
    /// Simplified content in english
    /// </summary>
    [JsonPropertyName("contentInEnglish")]
    public string ContentInEnglish { get; set; } = string.Empty;

    /// <summary>
    /// Links from content
    /// </summary>
    [JsonPropertyName("links")]
    public EquatableList<Link> Links { get; set; } = [];

    /// <summary>
    /// Persons mentioned in content
    /// </summary>
    [JsonPropertyName("persons")]
    public EquatableList<Person> Persons { get; set; } = [];

    /// <summary>
    /// AI analysis results
    /// </summary>
    [JsonPropertyName("analysis")]
    public Analysis Analysis { get; set; } = new();
}


