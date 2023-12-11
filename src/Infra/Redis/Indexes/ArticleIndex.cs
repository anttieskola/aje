
namespace AJE.Infra.Redis.Indexes;

public class ArticleIndex : IRedisIndex
{
    public int Version => 3;

    public string Name => "idx:article";

    public string Prefix => "article:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    // remember space at the end of lines
    // numeric fields can't be made TAGs (index breaks)
    // when we add new fields to model and adjust index data needs to be updated/reloaded as we only find keys that contain default value
    public string Schema => "$.id AS id TAG "
        + "$.category AS category NUMERIC "
        + "$.title AS title TEXT "
        + "$.modified AS modified NUMERIC SORTABLE "
        + "$.published AS published TAG "
        + "$.source AS source TAG "
        + "$.language AS language TAG "
        + "$.polarity AS polarity NUMERIC "
        + "$.polarityVersion AS polarityVersion NUMERIC "
        + "$.isValidated AS isValidated TAG "
        + "$.tokenCount AS tokenCount NUMERIC "
        + "$.articleToneVersion AS articleToneVersion NUMERIC "
        + "$.credibilityScoreVersion AS credibilityScoreVersion NUMERIC "
        + "$.honestyScoreVersion AS honestyScoreVersion NUMERIC "
        + "$.sensationalismScoreVersion AS sensationalismScoreVersion NUMERIC "
        + "$.empathyScoreVersion AS empathyScoreVersion NUMERIC "
        + "$.racismScoreVersion AS racismScoreVersion NUMERIC "
        + "$.criminalityScoreVersion AS criminalityScoreVersion NUMERIC "
        + "$.biasWesternScoreVersion AS biasWesternScoreVersion NUMERIC "
        + "$.biasEasternScoreVersion AS biasEasternScoreVersion NUMERIC "
        + "$.religionScoreVersion AS religionScoreVersion NUMERIC "
        + "$.christianityScoreVersion AS christianityScoreVersion NUMERIC "
        + "$.islamScoreVersion AS islamScoreVersion NUMERIC "
        + "$.hinduismScoreVersion AS hinduismScoreVersion NUMERIC "
        + "$.buddhismScoreVersion AS buddhismScoreVersion NUMERIC "
        + "$.judaismScoreVersion AS judaismScoreVersion NUMERIC";


    public RedisChannel Channel =>
        new("articles", RedisChannel.PatternMode.Auto);
}
