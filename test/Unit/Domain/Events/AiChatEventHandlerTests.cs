using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Events;

/// <summary>
/// Tests purpose is mainly for design of the event handler interface
/// There is no actuall implementation of the event handler in unit tests
/// </summary>
public class AiChatEventHandlerTests
{
    private class TestAiChatEventHandler : IAiChatEventHandler
    {
        public Task SendAsync(AiChatEvent aiChatEvent)
        {
            foreach (var handler in _handlers)
            {
                handler(aiChatEvent);
            }
            if (_chatHandlers.TryGetValue(aiChatEvent.ChatId, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(aiChatEvent);
                }
            }
            return Task.CompletedTask;
        }

        private readonly List<Action<AiChatEvent>> _handlers = new();
        private readonly Dictionary<Guid, List<Action<AiChatEvent>>> _chatHandlers = new();

        public void Subscribe(Action<AiChatEvent> handler)
        {
            _handlers.Add(handler);
        }

        public void Subscribe(Guid chatId, Action<AiChatEvent> handler)
        {
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
    }

    [Fact]
    public void SubscribeAll()
    {
        // arrange
        var events = new List<AiChatEvent>();
        void Handler(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(events);
            events.Add(aiChatEvent);
        }

        var handler = new TestAiChatEventHandler();
        handler.Subscribe(Handler);

        // act
        var chatEvent = new AiChatStartedEvent
        {
            IsTest = true,
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        handler.SendAsync(chatEvent);

        // assert
        Assert.Single(events);
        Assert.Equal(chatEvent, events[0]);

        // unsubscribe test
        events.Clear();
        handler.Unsubscribe(Handler);
        handler.SendAsync(chatEvent);
        Assert.Empty(events);
    }

    [Fact]
    public void ManySubscribers()
    {
        // arrange
        var handlerOneEvents = new List<AiChatEvent>();
        void HandlerOne(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(handlerOneEvents);
            handlerOneEvents.Add(aiChatEvent);
        }

        var handlerTwoEvents = new List<AiChatEvent>();
        void HandlerTwo(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(handlerTwoEvents);
            handlerTwoEvents.Add(aiChatEvent);
        }

        // act
        var handler = new TestAiChatEventHandler();
        handler.Subscribe(HandlerOne);
        handler.Subscribe(HandlerTwo);
        var chatEvent = new AiChatStartedEvent
        {
            IsTest = true,
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        handler.SendAsync(chatEvent);

        // assert
        Assert.Single(handlerOneEvents);
        Assert.Equal(chatEvent, handlerOneEvents[0]);
        Assert.Single(handlerTwoEvents);
        Assert.Equal(chatEvent, handlerTwoEvents[0]);

        // unsubscribe test
        handlerOneEvents.Clear();
        handler.Unsubscribe(HandlerOne);
        handler.SendAsync(chatEvent);
        Assert.Empty(handlerOneEvents);
        Assert.Equal(2, handlerTwoEvents.Count);
    }

    [Fact]
    public void SubscribeChat()
    {
        // arrange
        var events = new List<AiChatEvent>();
        void Handler(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(events);
            events.Add(aiChatEvent);
        }

        var chatId = Guid.NewGuid();
        var chatStartTimestamp = DateTimeOffset.UtcNow;
        var handler = new TestAiChatEventHandler();
        handler.Subscribe(chatId, Handler);

        // act - start
        var chatEvent = new AiChatStartedEvent
        {
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
        };
        handler.SendAsync(chatEvent);
        // act - interaction
        var chatInteractionEvent = new AiChatInteractionEvent
        {
            IsTest = true,
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
            InteractionId = Guid.NewGuid(),
            InteractionTimestamp = DateTimeOffset.UtcNow,
            Input = "input",
            Output = "output",
        };
        handler.SendAsync(chatInteractionEvent);

        // assert
        Assert.Equal(2, events.Count);
        Assert.Equal(chatEvent, events[0]);
        Assert.Equal(chatInteractionEvent, events[1]);

        // different chat test
        handler.SendAsync(new AiChatStartedEvent
        {
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        });
        Assert.Equal(2, events.Count);

        // unsubscribe test
        events.Clear();
        handler.Unsubscribe(Handler);
        handler.SendAsync(chatEvent);
        Assert.Empty(events);
    }

    [Fact]
    public void SubscribeManyChat()
    {
        // arrange
        var handlerAllEvents = new List<AiChatEvent>();
        void HandlerAll(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(handlerAllEvents);
            handlerAllEvents.Add(aiChatEvent);
        }

        var handlerOneEvents = new List<AiChatEvent>();
        void HandlerOne(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(handlerOneEvents);
            handlerOneEvents.Add(aiChatEvent);
        }

        var handlerTwoEvents = new List<AiChatEvent>();
        void HandlerTwo(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(handlerTwoEvents);
            handlerTwoEvents.Add(aiChatEvent);
        }

        var chatId = Guid.NewGuid();
        var chatStartTimestamp = DateTimeOffset.UtcNow;
        var handler = new TestAiChatEventHandler();
        handler.Subscribe(HandlerAll);
        handler.Subscribe(chatId, HandlerOne);
        handler.Subscribe(chatId, HandlerTwo);

        // act - start
        var chatEvent = new AiChatStartedEvent
        {
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
        };
        handler.SendAsync(chatEvent);

        // unsubbing chat id handler two
        handler.Unsubscribe(HandlerTwo);

        // act - interaction
        var chatInteractionEvent = new AiChatInteractionEvent
        {
            IsTest = true,
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
            InteractionId = Guid.NewGuid(),
            InteractionTimestamp = DateTimeOffset.UtcNow,
            Input = "input",
            Output = "output",
        };
        handler.SendAsync(chatInteractionEvent);

        // assert
        Assert.Equal(2, handlerAllEvents.Count);
        Assert.Equal(2, handlerOneEvents.Count);
        Assert.Single(handlerTwoEvents);

        // different chat test
        handler.SendAsync(new AiChatStartedEvent
        {
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        });
        Assert.Equal(3, handlerAllEvents.Count);
        Assert.Equal(2, handlerOneEvents.Count);
        Assert.Single(handlerTwoEvents);
    }
}
