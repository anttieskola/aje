
namespace AJE.Application.Indexes;

public class ArticleIndex : IRedisIndex
{
    public string Name => "idx:article";

    public string Prefix => "article:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.id AS id TAG "
        + "$.category AS category NUMERIC "
        + "$.title AS title TEXT "
        + "$.modified AS modified NUMERIC SORTABLE "
        + "$.published AS published TAG "
        + "$.source AS source TAG "
        + "$.language AS language TAG";

    public RedisChannel Channel =>
        new("articles", RedisChannel.PatternMode.Auto);
}
