namespace AJE.Infra.Redis.Indexes;

public interface IRedisIndex
{
    int Version { get; }
    string Name { get; }
    string Prefix { get; }
    string RedisId(string itemId);
    string Schema { get; }
    RedisChannel Channel { get; }
}