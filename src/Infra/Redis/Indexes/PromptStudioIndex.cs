
namespace AJE.Infra.Redis.Indexes;

public class PromptStudioIndex : IRedisIndex
{
    public int Version => 2;

    public string Name => "idx:promptstudio";

    public string Prefix => "promptstudio:";

    public string RedisId(string itemId) => $"{Prefix}{itemId}";

    public string Schema => "$.sessionId AS sessionId TAG "
        + "$.title AS title TEXT "
        + "$.modified AS modified NUMERIC SORTABLE";

    public RedisChannel Channel =>
        new("promptstudio", RedisChannel.PatternMode.Auto);
}
