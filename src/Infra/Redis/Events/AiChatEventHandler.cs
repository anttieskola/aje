
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
        await Task.Delay(TimeSpan.FromMilliseconds(10)); // this is a hack to make sure the message is sent before the test continues
    }

    private bool _subscribed = false;

    private readonly List<Action<AiChatEvent>> _handlers = new();
    private readonly Dictionary<Guid, List<Action<AiChatEvent>>> _chatHandlers = new();

    private void Subscribe()
    {
        if (!_subscribed)
        {
            var subscriber = _connection.GetSubscriber();
            subscriber.SubscribeAsync(_index.Channel, HandleMessage);
            _subscribed = true;
        }
    }

    public void Subscribe(Action<AiChatEvent> handler)
    {
        Subscribe();
        _handlers.Add(handler);
    }

    public void Subscribe(Guid chatId, Action<AiChatEvent> handler)
    {
        Subscribe();
        if (_chatHandlers.TryGetValue(chatId, out var handlers))
        {
            handlers.Add(handler);
        }
        else
        {
            _chatHandlers.Add(chatId, new List<Action<AiChatEvent>> { handler });
        }
    }

    public void Unsubscribe(Action<AiChatEvent> handler)
    {
        if (_handlers.Contains(handler))
            _handlers.Remove(handler);

        if (_chatHandlers.Values.Any(handlers => handlers.Contains(handler)))
            _chatHandlers.Values.First(handlers => handlers.Contains(handler)).Remove(handler);
    }

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
                var chatId = msg.ChatId;
                if (_chatHandlers.TryGetValue(chatId, out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        handler(msg);
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deserialize message");
        }
    }
}
