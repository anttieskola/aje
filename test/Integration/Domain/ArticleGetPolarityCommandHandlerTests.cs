using System.Text.Json;
using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

/// <summary>
/// Tests require a llama.cpp server running in localhost:8080
/// </summary>
#pragma warning disable xUnit1033
[Collection("Llama")]
public class ArticleGetPolarityCommandHandlerTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033
    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;
    private readonly LlamaQueueFixture _llamaQueueFixture;

    public ArticleGetPolarityCommandHandlerTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture,
        LlamaQueueFixture llamaQueueFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
        _llamaQueueFixture = llamaQueueFixture;
    }

    #region json
    private static string _article = @"
{
    ""category"": 2,
    ""modified"": 638333882240000000,
    ""published"": true,
    ""source"": ""https://yle.fi/a/74-20056194"",
    ""language"": ""en"",
    ""content"": [
        {
            ""$type"": ""header"",
            ""level"": 1,
            ""text"": ""Government aims to legalise alcohol home deliveries""
        },
        {
            ""$type"": ""text"",
            ""text"": ""The government is planning to loosen a number of restrictions on booze sales.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""Consumers will be able to have alcoholic drinks delivered to their homes from shops and restaurants, if the government's plans are implemented.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""Prime Minister **Petteri Orpo**'s (NCP) government is aiming for alcoholic beverages to be available to order from any retailer with a licence, including shops, kiosks and restaurants.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""Stronger drinks would remain restricted to the state alcohol monopoly Alko, who would also get the right to bring in home deliveries for their own products, such as wines.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""The upper limit on alcohol strength for other retailers is slated to increase to eight percent from spring 2024.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""In addition, the government is preparing legislation to expand the rights of breweries and wineries to sell their products directly to consumers.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""The current limit for producers to sell direct to consumers is 13 percent strength drinks, but the government is proposing an increase on that. In addition, they would be allowed to introduce mail order services.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""The Ministry for Social Affairs and Health is preparing the proposal, and says the most challenging aspect is ensuring that age restrictions on alcohol sales are enforced properly.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""The Federation of the Brewing and Soft Drinks Industry has proposed that home deliveries would be allowed between the hours of 9am and 9pm.""
        },
        {
            ""$type"": ""text"",
            ""text"": ""***Users with an Yle ID can leave comments on our news stories. You can create your Yle ID via*** [***this link***](https://yle.fi/aihe/yle-tunnus/yle-id)***. Our guidelines on commenting, including moderation are explained in*** [***this article***](https://yle.fi/a/74-20054430)***. You can comment on this article until 23:00 on 21 October.***""
        }
    ],
    ""chat"": [],
    ""id"": ""0a0eb2f7-f4db-4e6a-8617-b905e35e1aa7"",
    ""title"": ""Government aims to legalise alcohol home deliveries""
}";

    #endregion json

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
        var article = JsonSerializer.Deserialize<Article>(_article);
        Assert.NotNull(article);
        var handler = new ArticleGetSentimentPolarityQueryHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            new PolarityChatML(),
            new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true),
            new AiLogger(new Mock<ILogger<AiLogger>>().Object, TestConstants.LlamaConfiguration));
        var command = new ArticleGetSentimentPolarityQuery { Article = article };
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(response);
    }
}
