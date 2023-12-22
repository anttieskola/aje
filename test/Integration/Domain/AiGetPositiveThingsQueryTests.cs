using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetPositiveThingsQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetPositiveThingsQueryTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture,
        LlamaQueueFixture llamaQueueFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
        // llamaQueueFixture must exist
    }

    private IServiceProvider CreateMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LlamaApi>))).Returns(new Mock<ILogger<LlamaApi>>().Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IHttpClientFactory))).Returns(_httpClientFixture.HttpClientFactory);
        return mockServiceProvider.Object;
    }

    [Fact]
    public async Task Ok()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetPositiveThingsQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetPositiveThingsQuery { Context = "I like cats\nI love dogs\n" }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Contains("cats", response);
        Assert.Contains("dogs", response);
    }

    [Fact]
    public async Task Hate()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetPositiveThingsQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetPositiveThingsQuery { Context = "I hate everything" }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }
}
