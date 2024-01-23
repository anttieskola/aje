
namespace AJE.Infra.Redis.Indexes;

public class AiStoryIndex : IRedisIndex
{
    public int Version => 1;

    public string Name => "idx:aistory";

    public string Prefix => "aistory:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.storyId AS id TAG";

    public RedisChannel Channel => new("aistory", RedisChannel.PatternMode.Auto);

}
