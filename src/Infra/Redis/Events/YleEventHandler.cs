namespace AJE.Infra.Redis.Events;

public class YleEventHandler : IYleEventHandler
{
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisChannel _channel;

    public YleEventHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
        _channel = new RedisChannel("yle", RedisChannel.PatternMode.Auto);
    }

    public async Task SendAsync(YleEvent YleEvent)
    {
        var db = _connection.GetDatabase();
        await db.PublishAsync(_channel, JsonSerializer.Serialize(YleEvent));
    }
}
