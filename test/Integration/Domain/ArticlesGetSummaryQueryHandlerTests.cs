using AJE.Domain.Queries;
using AJE.Infra.Ai;
using AJE.Infra.Redis.Data;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class ArticlesGetSummaryQueryHandlerTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033
    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;
    private readonly LlamaQueueFixture _llamaQueueFixture;

    public ArticlesGetSummaryQueryHandlerTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture,
        LlamaQueueFixture llamaQueueFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
        _llamaQueueFixture = llamaQueueFixture;
    }

    private IServiceProvider CreateMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LlamaApi>))).Returns(new Mock<ILogger<LlamaApi>>().Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IHttpClientFactory))).Returns(_httpClientFixture.HttpClientFactory);
        return mockServiceProvider.Object;
    }

    [Fact(Skip = "This is just a test to see if this could work")]
    public async Task SummaryOfSummaries()
    {
        var articleRepository = new ArticleRepository(new Mock<ILogger<ArticleRepository>>().Object, _redisFixture.Connection);
        var articleGetManyQueryHandler = new ArticleGetManyQueryHandler(articleRepository);
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var articlesGetSummaryQueryHandler = new ArticlesGetSummaryQueryHandler(aiModel);

        var queryResponse = await articleGetManyQueryHandler.Handle(new ArticleGetManyQuery
        {
            Offset = 0,
            PageSize = 80,
        }, CancellationToken.None);

        var summary = await articlesGetSummaryQueryHandler.Handle(new ArticlesGetSummaryQuery
        {
            Articles = queryResponse.Items,
        }, CancellationToken.None);
        Assert.NotNull(summary);
    }
}