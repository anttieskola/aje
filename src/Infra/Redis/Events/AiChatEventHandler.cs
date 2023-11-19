
using System.Collections.Concurrent;

namespace AJE.Infra.Redis.Events;

public class AiChatEventHandler : IAiChatEventHandler
{
    private readonly ILogger<AiChatEventHandler> _logger;
    private readonly IRedisIndex _index = new AiChatIndex();
    private readonly IConnectionMultiplexer _connection;
    public AiChatEventHandler(
        ILogger<AiChatEventHandler> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task SendAsync(AiChatEvent aiChatEvent)
    {
        var db = _connection.GetDatabase();
        await db.PublishAsync(_index.Channel, JsonSerializer.Serialize(aiChatEvent));
    }

    private bool _subscribed = false;
    private ConcurrentQueue<Action<AiChatEvent>> _handlers = new();

    private void HandleMessage(RedisChannel channel, RedisValue value)
    {
        try
        {
            var msg = JsonSerializer.Deserialize<AiChatEvent>(value.ToString());
            if (msg == null)
            {
                _logger.LogError("Failed to deserialize message:{}", value);
            }
            else
            {
                foreach (var handler in _handlers)
                {
                    handler(msg);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deserialize message");
        }
    }

    public void Subscribe(Action<AiChatEvent> handler)
    {
        if (!_subscribed)
        {
            var subscriber = _connection.GetSubscriber();
            subscriber.SubscribeAsync(_index.Channel, HandleMessage);
        }
        _handlers.Enqueue(handler);
    }

    public void Subscribe(Guid chatId, Action<AiChatEvent> handler)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(Action<AiChatEvent> handler)
    {
        throw new NotImplementedException();
    }
}
