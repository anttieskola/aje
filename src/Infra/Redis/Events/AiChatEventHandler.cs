﻿namespace AJE.Infra.Redis.Events;

public class AiChatEventHandler : IAiChatEventHandler, IDisposable
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

    private Thread? _deliveryThread;
    private readonly object _lock = new();
    private void Subscribe()
    {
        lock (_lock)
        {
            if (_deliveryThread == null)
            {
                _deliveryThread = new Thread(() =>
                {
                    var subscriber = _connection.GetSubscriber();
                    subscriber.SubscribeAsync(_index.Channel, HandleMessage);
                })
                {
                    IsBackground = true,
                    Name = "AiChatEventHandlerSubscription",
                };
                _deliveryThread.Start();
            }
        }
    }

    private readonly ConcurrentDictionary<Guid, Func<AiChatEvent, Task>> _handlers = new();

    public void Subscribe(Guid subscriberId, Func<AiChatEvent, Task> handler)
    {
        Subscribe();
        if (!_handlers.ContainsKey(subscriberId))
        {
            if (!_handlers.TryAdd(subscriberId, handler))
            {
                throw new SubscriptionException($"Failed to even subscriber with id:{subscriberId}");
            }
        }
    }

    private class ChatSubscriber
    {
        public required Guid ChatId { get; init; }
        public required Func<AiChatEvent, Task> Handler { get; init; }
    }

    private readonly ConcurrentDictionary<Guid, ChatSubscriber> _chatEventhandlers = new();

    public void SubscribeToChat(Guid subscriberId, Guid chatId, Func<AiChatEvent, Task> handler)
    {
        Subscribe();
        if (!_chatEventhandlers.ContainsKey(subscriberId))
        {
            var cb = new ChatSubscriber { ChatId = chatId, Handler = handler };
            if (!_chatEventhandlers.TryAdd(subscriberId, cb))
            {
                throw new SubscriptionException($"Failed to even subscriber with id:{subscriberId}");
            }
        }
    }

    public void Unsubscribe(Guid subscriberId)
    {
        if (_handlers.ContainsKey(subscriberId))
        {
            if (!_handlers.TryRemove(subscriberId, out _))
            {
                throw new SubscriptionException($"Failed to remove subscriber with id:{subscriberId}");
            }
        }
        if (_chatEventhandlers.ContainsKey(subscriberId))
        {
            if (!_chatEventhandlers.TryRemove(subscriberId, out _))
            {
                throw new SubscriptionException($"Failed to remove chat subscriber with id:{subscriberId}");
            }
        }
    }

    private void HandleMessage(RedisChannel channel, RedisValue value)
    {
        _logger.LogDebug("HandleMessage {}", DateTime.UtcNow);
        try
        {
            var msg = JsonSerializer.Deserialize<AiChatEvent>(value.ToString());
            if (msg == null)
            {
                _logger.LogError("Channel:{}, Contains empty message:{}", channel, value);
            }
            else
            {
                foreach (var handler in _handlers.Values)
                {
                    handler(msg).Wait();
                }

                var chatId = msg.ChatId;
                foreach (var handler in _chatEventhandlers.Values.Where(x => x.ChatId == chatId))
                {
                    handler.Handler(msg).Wait();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Channel:{}, Failed to deserialize message:{}", channel, value);
        }
    }

    #region IDisposable Support

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_deliveryThread != null)
                {
                    _deliveryThread.Join();
                    _deliveryThread = null;
                }
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
}
