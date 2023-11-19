using AJE.Domain.Events;
using AJE.Infra.Redis.Events;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Infra;

public class AiChatEventHandlerTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly IRedisIndex _index = new AiChatIndex();
    public AiChatEventHandlerTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    [Fact]
    public async Task Subscribe()
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

        // TODO
        Assert.True(true);
    }
}
