namespace AJE.Domain.Entities;

/// <summary>
/// Collection of silly AI analysis ideas to score on
/// </summary>
public record Analysis
{
    [JsonPropertyName("summaryVersion")]
    public int SummaryVersion { get; set; } = 0;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("positiveThingsVersion")]
    public int PositiveThingsVersion { get; set; } = 0;

    [JsonPropertyName("positiveThings")]
    public string PositiveThings { get; set; } = string.Empty;

    [JsonPropertyName("locationsVersion")]
    public int LocationsVersion { get; set; } = 0;

    [JsonPropertyName("locations")]
    public EquatableList<Location> Locations { get; set; } = [];

    [JsonPropertyName("corporationsVersion")]
    public int CorporationsVersion { get; set; } = 0;

    [JsonPropertyName("corporations")]
    public EquatableList<Corporation> Corporations { get; set; } = [];

    [JsonPropertyName("organizationsVersion")]
    public int OrganizationsVersion { get; set; } = 0;

    [JsonPropertyName("organizations")]
    public EquatableList<Organization> Organizations { get; set; } = [];

    #region stupid ideas

    [JsonPropertyName("sentimentScoreVersion")]
    public int SentimentScoreVersion { get; set; } = 0;

    [JsonPropertyName("sentimentScore")]
    public double SentimentScore { get; set; } = 0.0;

    [JsonPropertyName("sentimentScoreReasoning")]
    public string SentimentScoreReasoning { get; set; } = string.Empty;

    [JsonPropertyName("newsTypeVersion")]
    public int NewsTypeVersion { get; set; } = 0;

    [JsonPropertyName("newsType")]
    public NewsArticleType NewsType { get; set; } = NewsArticleType.Unknown;

    [JsonPropertyName("newsTypeReasoning")]
    public string NewsTypeReasoning { get; set; } = string.Empty;

    [JsonPropertyName("articleToneVersion")]
    public int ArticleToneVersion { get; set; } = 0;

    [JsonPropertyName("articleTone")]
    public ArticleTone ArticleTone { get; set; } = ArticleTone.Unknown;

    [JsonPropertyName("articleToneReasoning")]
    public string ArticleToneReasoning { get; set; } = string.Empty;

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

    #endregion stupid ideas
}