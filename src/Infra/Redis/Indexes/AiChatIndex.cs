
namespace AJE.Infra.Redis.Indexes;

public class AiChatIndex : IRedisIndex
{
    public int Version => 1;

    public string Name => "idx:aichat";

    public string Prefix => "aichat:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.id AS id TAG";

    public RedisChannel Channel =>
        new("aichat", RedisChannel.PatternMode.Auto);
}
