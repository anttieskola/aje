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

    private readonly TimeSpan _msgWaitTimeout = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task SingleSubsriber()
    {
        // arrange
        var subscriberId = Guid.NewGuid();
        var events = new List<AiChatEvent>();
        async Task Handler(AiChatEvent aiChatEvent)
        {
            await Task.Delay(TimeSpan.FromMicroseconds(1));
            Assert.NotNull(events);
            events.Add(aiChatEvent);
        }

        using var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
        handler.Subscribe(subscriberId, Handler);

        // act
        var chatId = Guid.NewGuid();
        var chatTimestamp = DateTimeOffset.UtcNow;
        var startEvent = new AiChatStartedEvent
        {
            IsTest = true,
            ChatId = chatId,
            StartTimestamp = chatTimestamp,
        };
        await handler.SendAsync(startEvent);
        await Task.Delay(_msgWaitTimeout);

        // assert start event
        Assert.Single(events);
        Assert.Equal(startEvent, events[0]);

        // act interaction event
        var interactionEvent = new AiChatInteractionEvent
        {
            IsTest = true,
            ChatId = chatId,
            StartTimestamp = chatTimestamp,
            InteractionId = Guid.NewGuid(),
            InteractionTimestamp = DateTimeOffset.UtcNow,
            Input = "input",
            Output = "output",
        };
        await handler.SendAsync(interactionEvent);
        await Task.Delay(_msgWaitTimeout);

        // assert interaction event
        Assert.Equal(2, events.Count);
        var se = events.Single(e => e.GetType() == typeof(AiChatStartedEvent));
        Assert.Equal(startEvent, se);
        var ie = events.Single(e => e.GetType() == typeof(AiChatInteractionEvent));
        Assert.Equal(interactionEvent, ie);

        // unsubscribe test
        handler.Unsubscribe(subscriberId);
        await handler.SendAsync(startEvent);
        await handler.SendAsync(interactionEvent);
        Assert.Equal(2, events.Count);
        se = events.Single(e => e.GetType() == typeof(AiChatStartedEvent));
        Assert.Equal(startEvent, se);
        ie = events.Single(e => e.GetType() == typeof(AiChatInteractionEvent));
        Assert.Equal(interactionEvent, ie);
        events.Clear();
        await handler.SendAsync(startEvent);
        await handler.SendAsync(interactionEvent);
        Assert.Empty(events);
    }

    // [Fact]
    // public async Task ManySubscribers()
    // {
    //     // arrange
    //     var handlerOneEvents = new List<AiChatEvent>();
    //     void HandlerOne(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(handlerOneEvents);
    //         handlerOneEvents.Add(aiChatEvent);
    //     }

    //     var handlerTwoEvents = new List<AiChatEvent>();
    //     void HandlerTwo(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(handlerTwoEvents);
    //         handlerTwoEvents.Add(aiChatEvent);
    //     }

    //     // act
    //     var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
    //     handler.Subscribe(HandlerOne);
    //     handler.Subscribe(HandlerTwo);
    //     var chatEvent = new AiChatStartedEvent
    //     {
    //         IsTest = true,
    //         ChatId = Guid.NewGuid(),
    //         StartTimestamp = DateTimeOffset.UtcNow,
    //     };
    //     await handler.SendAsync(chatEvent);

    //     // assert
    //     Assert.Single(handlerOneEvents);
    //     Assert.Equal(chatEvent, handlerOneEvents[0]);
    //     Assert.Single(handlerTwoEvents);
    //     Assert.Equal(chatEvent, handlerTwoEvents[0]);

    //     // unsubscribe test
    //     handlerOneEvents.Clear();
    //     handler.Unsubscribe(HandlerOne);
    //     await handler.SendAsync(chatEvent);
    //     Assert.Empty(handlerOneEvents);
    //     Assert.Equal(2, handlerTwoEvents.Count);
    // }

    // [Fact]
    // public async Task SubscribeChat()
    // {
    //     // arrange
    //     var events = new List<AiChatEvent>();
    //     void Handler(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(events);
    //         events.Add(aiChatEvent);
    //     }

    //     var chatId = Guid.NewGuid();
    //     var chatStartTimestamp = DateTimeOffset.UtcNow;
    //     var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
    //     handler.Subscribe(chatId, Handler);

    //     // act - start
    //     var chatEvent = new AiChatStartedEvent
    //     {
    //         ChatId = chatId,
    //         StartTimestamp = chatStartTimestamp,
    //     };
    //     await handler.SendAsync(chatEvent);
    //     // act - interaction
    //     var chatInteractionEvent = new AiChatInteractionEvent
    //     {
    //         IsTest = true,
    //         ChatId = chatId,
    //         StartTimestamp = chatStartTimestamp,
    //         InteractionId = Guid.NewGuid(),
    //         InteractionTimestamp = DateTimeOffset.UtcNow,
    //         Input = "input",
    //         Output = "output",
    //     };
    //     await handler.SendAsync(chatInteractionEvent);

    //     // assert
    //     Assert.Equal(2, events.Count);
    //     Assert.Equal(chatEvent, events[0]);
    //     Assert.Equal(chatInteractionEvent, events[1]);

    //     // different chat test
    //     await handler.SendAsync(new AiChatStartedEvent
    //     {
    //         ChatId = Guid.NewGuid(),
    //         StartTimestamp = DateTimeOffset.UtcNow,
    //     });
    //     Assert.Equal(2, events.Count);

    //     // unsubscribe test
    //     events.Clear();
    //     handler.Unsubscribe(Handler);
    //     await handler.SendAsync(chatEvent);
    //     Assert.Empty(events);
    // }

    // [Fact]
    // public async Task SubscribeManyChat()
    // {
    //     // arrange
    //     var handlerAllEvents = new List<AiChatEvent>();
    //     void HandlerAll(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(handlerAllEvents);
    //         handlerAllEvents.Add(aiChatEvent);
    //     }

    //     var handlerOneEvents = new List<AiChatEvent>();
    //     void HandlerOne(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(handlerOneEvents);
    //         handlerOneEvents.Add(aiChatEvent);
    //     }

    //     var handlerTwoEvents = new List<AiChatEvent>();
    //     void HandlerTwo(AiChatEvent aiChatEvent)
    //     {
    //         Assert.NotNull(handlerTwoEvents);
    //         handlerTwoEvents.Add(aiChatEvent);
    //     }

    //     var chatId = Guid.NewGuid();
    //     var chatStartTimestamp = DateTimeOffset.UtcNow;
    //     var handler = new AiChatEventHandler(new Mock<ILogger<AiChatEventHandler>>().Object, _redisFixture.Connection);
    //     handler.Subscribe(HandlerAll);
    //     handler.Subscribe(chatId, HandlerOne);
    //     handler.Subscribe(chatId, HandlerTwo);

    //     // act - start
    //     var chatEvent = new AiChatStartedEvent
    //     {
    //         ChatId = chatId,
    //         StartTimestamp = chatStartTimestamp,
    //     };
    //     await handler.SendAsync(chatEvent);

    //     // unsubbing chat id handler two
    //     handler.Unsubscribe(HandlerTwo);

    //     // act - interaction
    //     var chatInteractionEvent = new AiChatInteractionEvent
    //     {
    //         IsTest = true,
    //         ChatId = chatId,
    //         StartTimestamp = chatStartTimestamp,
    //         InteractionId = Guid.NewGuid(),
    //         InteractionTimestamp = DateTimeOffset.UtcNow,
    //         Input = "input",
    //         Output = "output",
    //     };
    //     await handler.SendAsync(chatInteractionEvent);

    //     // assert
    //     Assert.Equal(2, handlerAllEvents.Count);
    //     Assert.Equal(2, handlerOneEvents.Count);
    //     Assert.Single(handlerTwoEvents);

    //     // different chat test
    //     await handler.SendAsync(new AiChatStartedEvent
    //     {
    //         ChatId = Guid.NewGuid(),
    //         StartTimestamp = DateTimeOffset.UtcNow,
    //     });
    //     Assert.Equal(3, handlerAllEvents.Count);
    //     Assert.Equal(2, handlerOneEvents.Count);
    //     Assert.Single(handlerTwoEvents);
    // }
}
