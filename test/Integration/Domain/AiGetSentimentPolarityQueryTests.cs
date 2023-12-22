using AJE.Domain.Enums;
using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetSentimentPolarityQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetSentimentPolarityQueryTests(
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
    public async Task Positive()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetSentimentPolarityQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetSentimentPolarityQuery { Context = "It's christmas and everyones on cheerfull and singing" }, CancellationToken.None);
        Assert.Equal(Polarity.Positive, response);
    }

    [Fact]
    public async Task Neutral()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetSentimentPolarityQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetSentimentPolarityQuery { Context = "Goverment will gather to discuss the future" }, CancellationToken.None);
        Assert.Equal(Polarity.Neutral, response);
    }

    [Fact]
    public async Task Negative()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetSentimentPolarityQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetSentimentPolarityQuery { Context = "Gas prices are skyrocketing" }, CancellationToken.None);
        Assert.Equal(Polarity.Negative, response);
    }
}
