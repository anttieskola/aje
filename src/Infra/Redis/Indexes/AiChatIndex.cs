
namespace AJE.Infra.Redis.Indexes;

public class AiChatIndex : IRedisIndex
{
    public int Version => 2;

    public string Name => "idx:aichat";

    public string Prefix => "aichat:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.chatId AS chatId TAG";

    public RedisChannel Channel =>
        new("aichat", RedisChannel.PatternMode.Auto);
}
