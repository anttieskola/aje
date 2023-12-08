
namespace AJE.Infra.Redis.Indexes;

public class PromptStudioIndex : IRedisIndex
{
    public int Version => 1;

    public string Name => "idx:promptstudio";

    public string Prefix => "promptstudio:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.id AS id TAG";

    public RedisChannel Channel =>
        new("promptstudio", RedisChannel.PatternMode.Auto);
}
