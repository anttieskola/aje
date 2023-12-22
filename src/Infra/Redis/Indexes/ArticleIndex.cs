
namespace AJE.Infra.Redis.Indexes;

public class ArticleIndex : IRedisIndex
{
    public int Version => 6;

    public string Name => "idx:article";

    public string Prefix => "article:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    // remember space at the end of lines
    // numeric fields can't be made TAGs (index breaks)! forgot this again...
    // when we add new fields to model and adjust index data needs to be updated/reloaded as we only find keys that contain default value
    public string Schema => "$.id AS id TAG" + " "
        + "$.category AS category NUMERIC" + " "
        + "$.title AS title TEXT" + " "
        + "$.modified AS modified NUMERIC SORTABLE" + " "
        + "$.isLiveNews AS isLiveNews TAG" + " "
        + "$.source AS source TAG" + " "
        + "$.language AS language TAG" + " "
        + "$.polarity AS polarity NUMERIC" + " "
        + "$.polarityVersion AS polarityVersion NUMERIC" + " "
        + "$.isValidForAnalysis AS isValidForAnalysis TAG" + " "
        // analysis
        + "$.analysis.summaryVersion AS summaryVersion NUMERIC" + " "
        + "$.analysis.positiveThingsVersion AS positiveThingsVersion NUMERIC" + " "
        + "$.analysis.sentimentScoreVersion AS sentimentScoreVersion NUMERIC" + " "
        + "$.analysis.sentimentScore AS sentimentScore NUMERIC" + " "
        + "$.analysis.newsType AS newsType NUMERIC" + " "
        + "$.analysis.newsTypeVersion as newsTypeVersion NUMERIC" + " "
        + "$.analysis.articleToneVersion AS articleToneVersion NUMERIC" + " "
        + "$.analysis.credibilityScoreVersion AS credibilityScoreVersion NUMERIC" + " "
        + "$.analysis.honestyScoreVersion AS honestyScoreVersion NUMERIC" + " "
        + "$.analysis.sensationalismScoreVersion AS sensationalismScoreVersion NUMERIC" + " "
        + "$.analysis.empathyScoreVersion AS empathyScoreVersion NUMERIC" + " "
        + "$.analysis.racismScoreVersion AS racismScoreVersion NUMERIC" + " "
        + "$.analysis.criminalityScoreVersion AS criminalityScoreVersion NUMERIC" + " "
        + "$.analysis.biasWesternScoreVersion AS biasWesternScoreVersion NUMERIC" + " "
        + "$.analysis.biasEasternScoreVersion AS biasEasternScoreVersion NUMERIC" + " "
        + "$.analysis.religionScoreVersion AS religionScoreVersion NUMERIC";

    public RedisChannel Channel =>
        new("articles", RedisChannel.PatternMode.Auto);
}
