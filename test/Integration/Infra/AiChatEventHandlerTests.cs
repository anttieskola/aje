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

    // TODO TestingIsHard
    [Fact]
    public void TestingIsHard()
    {
        Assert.NotNull(_redisFixture);
    }
}
