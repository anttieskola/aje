using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetOrganizationsQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetOrganizationsQueryTests(
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

    private const string _context = @"
The Finnish Red Cross on the situation in Israel and Palestine: ""The situation in civils is very worrying.""
The Finnish Red Cross on the situation in Israel and Palestine Hamas, the Palestinian extremist organization,
made a massive terrorist attack on Israel on Saturday morning, after which Israel has bombed Gaza. Israel said
the airstrikes against Gaza are only the first extremist organization to launch a massacre in Israel this weekend.
The humanitarian situation is cata.phic for the international aid activities of the Finnish Red Cross,
Tiina Saarikoski, says the situation is very serious for civil. in Israel. The situation has been f.ile for
a long time. There are a lot of wounded people who need hospitalization. Health care is loaded, the Gaza area
and houses are closed, people need shelter, water and food, Saarikoski lists the most acute needs of assistance.
There are also a lot of people trying to get out of the area.";

    [Fact]
    public async Task Ok()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetOrganizationsQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetOrganizationsQuery { Context = _context }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response); // Red Cross, Hamas
    }
}
