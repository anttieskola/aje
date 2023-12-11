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
    public EquatableList<MarkdownElement> Content { get; set; } = EquatableList<MarkdownElement>.Empty;

    [JsonPropertyName("chat")]
    public EquatableList<ChatMessage> Chat { get; set; } = EquatableList<ChatMessage>.Empty;

    [JsonPropertyName("newsTypeVersion")]
    public int NewsTypeVersion { get; set; } = 0;

    [JsonPropertyName("newsType")]
    public NewsArticleType NewsType { get; set; } = NewsArticleType.Unknown;

    [JsonPropertyName("modified")]
    public long Modified { get; set; }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

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
    /// Number of tokens in the article, -1 means it has not been calculated yet
    /// </summary>
    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; set; } = -1;

    /// <summary>
    /// Is content validated to contain full article
    /// </summary>
    [JsonPropertyName("isValidated")]
    public bool IsValidated { get; set; } = false;

    [JsonPropertyName("articleToneVersion")]
    public int ArticleToneVersion { get; set; } = 0;

    /// <summary>
    /// Tone in which the article is written (AI)
    /// </summary>
    [JsonPropertyName("articleTone")]
    public ArticleTone ArticleTone { get; set; } = ArticleTone.Unknown;

    [JsonPropertyName("articleToneReasoning")]
    public string ArticleToneReasoning { get; set; } = string.Empty;

    #region Scoring different aspects of the article using AI

    [JsonPropertyName("credibilityScoreVersion")]
    public int CredibilityScoreVersion { get; set; } = 0;

    [JsonPropertyName("credibilityScore")]
    public double CredibilityScore { get; set; } = 0.0;

    [JsonPropertyName("credibilityScoreReasoning")]
    public string CredibilityScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("honestyScoreVersion")]
    public int HonestyScoreVersion { get; set; } = 0;

    [JsonPropertyName("honestyScore")]
    public double HonestyScore { get; set; } = 0.0;

    [JsonPropertyName("honestyScoreReasoning")]
    public string HonestyScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("sensationalismScoreVersion")]
    public int SensationalismScoreVersion { get; set; } = 0;

    [JsonPropertyName("sensationlismScore")]
    public double SensationalismScore { get; set; } = 0.0;

    [JsonPropertyName("sensationlismScoreReasoning")]
    public string SensationalismScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("empathyScoreVersion")]
    public int EmpathyScoreVersion { get; set; } = 0;

    [JsonPropertyName("empathyScore")]
    public double EmpathyScore { get; set; } = 0.0;

    [JsonPropertyName("empathyScoreReasoning")]
    public string EmpathyScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("racismScoreVersion")]
    public int RacismScoreVersion { get; set; } = 0;

    [JsonPropertyName("racismScore")]
    public double RacismScore { get; set; } = 0.0;

    [JsonPropertyName("criminalityScoreVersion")]
    public int CriminalityScoreVersion { get; set; } = 0;

    [JsonPropertyName("criminalityScore")]
    public double CriminalityScore { get; set; } = 0.0;

    [JsonPropertyName("criminalityScoreReasoning")]
    public string CriminalityScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("racismScoreReasoning")]
    public string RacismScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("biasWesternScoreVersion")]
    public int BiasWesternScoreVersion { get; set; } = 0;

    [JsonPropertyName("biasWesternScore")]
    public double BiasWesternScore { get; set; } = 0.0;

    [JsonPropertyName("biasWesternScoreReasoning")]
    public string BiasWesternScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("biasEasternScoreVersion")]
    public int BiasEasternScoreVersion { get; set; } = 0;

    [JsonPropertyName("biasEasternScore")]
    public double BiasEasternScore { get; set; } = 0.0;

    [JsonPropertyName("biasEasternScoreReasoning")]
    public string BiasEasternScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("religionScoreVersion")]
    public int ReligionScoreVersion { get; set; } = 0;

    [JsonPropertyName("religionScore")]
    public double ReligionScore { get; set; } = 0.0;

    [JsonPropertyName("religionScoreReasoning")]
    public string ReligionScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("christianityScoreVersion")]
    public int ChristianityScoreVersion { get; set; } = 0;

    [JsonPropertyName("christianityScore")]
    public double ChristianityScore { get; set; } = 0.0;

    [JsonPropertyName("christianityScoreReasoning")]
    public string ChristianityScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("islamScoreVersion")]
    public int IslamScoreVersion { get; set; } = 0;

    [JsonPropertyName("islamScore")]
    public double IslamScore { get; set; } = 0.0;

    [JsonPropertyName("islamScoreReasoning")]
    public string IslamScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("hinduismScoreVersion")]
    public int HinduismScoreVersion { get; set; } = 0;

    [JsonPropertyName("hinduismScore")]
    public double HinduismScore { get; set; } = 0.0;

    [JsonPropertyName("hinduismScoreReasoning")]
    public string HinduismScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("buddhismScoreVersion")]
    public int BuddhismScoreVersion { get; set; } = 0;

    [JsonPropertyName("buddhismScore")]
    public double BuddhismScore { get; set; } = 0.0;

    [JsonPropertyName("buddhismScoreReasoning")]
    public string BuddhismScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("judaismScoreVersion")]
    public int JudaismScoreVersion { get; set; } = 0;

    [JsonPropertyName("judaismScore")]
    public double JudaismScore { get; set; } = 0.0;

    [JsonPropertyName("judaismScoreReasoning")]
    public string JudaismScoreReasoning { get; set; } = string.Empty;

    #endregion Scoring different aspects of the article using AI
}
