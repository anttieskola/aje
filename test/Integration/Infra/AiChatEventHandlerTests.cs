using AJE.Domain.Events;
using AJE.Infra.Redis.Events;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Infra;

public class AiChatEventHandlerTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    public AiChatEventHandlerTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    [Fact]
    public async Task SubscribeAll()
    {
        // arrange
        var events = new List<AiChatEvent>();
        void Handler(AiChatEvent aiChatEvent)
        {
            Assert.NotNull(events);
            events.Add(aiChatEvent);
        }

        var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
        handler.Subscribe(Handler);

        // act
        var chatEvent = new AiChatStartedEvent
        {
            IsTest = true,
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        await handler.SendAsync(chatEvent);

        // assert
        Assert.Single(events);
        Assert.Equal(chatEvent, events[0]);

        // unsubscribe test
        events.Clear();
        handler.Unsubscribe(Handler);
        await handler.SendAsync(chatEvent);
        Assert.Empty(events);
    }

    [Fact]
    public async Task ManySubscribers()
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
        var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
        handler.Subscribe(HandlerOne);
        handler.Subscribe(HandlerTwo);
        var chatEvent = new AiChatStartedEvent
        {
            IsTest = true,
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        await handler.SendAsync(chatEvent);

        // assert
        Assert.Single(handlerOneEvents);
        Assert.Equal(chatEvent, handlerOneEvents[0]);
        Assert.Single(handlerTwoEvents);
        Assert.Equal(chatEvent, handlerTwoEvents[0]);

        // unsubscribe test
        handlerOneEvents.Clear();
        handler.Unsubscribe(HandlerOne);
        await handler.SendAsync(chatEvent);
        Assert.Empty(handlerOneEvents);
        Assert.Equal(2, handlerTwoEvents.Count);
    }

    [Fact]
    public async Task SubscribeChat()
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
        var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
        handler.Subscribe(chatId, Handler);

        // act - start
        var chatEvent = new AiChatStartedEvent
        {
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
        };
        await handler.SendAsync(chatEvent);
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
        await handler.SendAsync(chatInteractionEvent);

        // assert
        Assert.Equal(2, events.Count);
        Assert.Equal(chatEvent, events[0]);
        Assert.Equal(chatInteractionEvent, events[1]);

        // different chat test
        await handler.SendAsync(new AiChatStartedEvent
        {
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        });
        Assert.Equal(2, events.Count);

        // unsubscribe test
        events.Clear();
        handler.Unsubscribe(Handler);
        await handler.SendAsync(chatEvent);
        Assert.Empty(events);
    }

    [Fact]
    public async Task SubscribeManyChat()
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
        var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
        handler.Subscribe(HandlerAll);
        handler.Subscribe(chatId, HandlerOne);
        handler.Subscribe(chatId, HandlerTwo);

        // act - start
        var chatEvent = new AiChatStartedEvent
        {
            ChatId = chatId,
            StartTimestamp = chatStartTimestamp,
        };
        await handler.SendAsync(chatEvent);

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
        await handler.SendAsync(chatInteractionEvent);

        // assert
        Assert.Equal(2, handlerAllEvents.Count);
        Assert.Equal(2, handlerOneEvents.Count);
        Assert.Single(handlerTwoEvents);

        // different chat test
        await handler.SendAsync(new AiChatStartedEvent
        {
            ChatId = Guid.NewGuid(),
            StartTimestamp = DateTimeOffset.UtcNow,
        });
        Assert.Equal(3, handlerAllEvents.Count);
        Assert.Equal(2, handlerOneEvents.Count);
        Assert.Single(handlerTwoEvents);
    }
}
