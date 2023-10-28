namespace AJE.Infra.Redis.Indexes;

public interface IRedisIndex
{
    string Name { get; }
    string Prefix { get; }
    string RedisId(string itemId);
    string Schema { get; }
    RedisChannel Channel { get; }
}