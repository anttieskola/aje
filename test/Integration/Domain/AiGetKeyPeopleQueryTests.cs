using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetKeyPeopleQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetKeyPeopleQueryTests(
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
German researchers: Large salt pulse in the Baltic Sea German scientists A large salt pulse in the Baltic
Sea may be the largest in a long time. It is expected to improve the Baltic Sea. A large salt pulse has
arrived in the Baltic Sea. It may even be as large as the salt pulse that arrived in December 2014,
according to the German marine research facility IOW. In Finland, the Finnish N. magazine was first
published on its website. Salt water has flown to the Baltic Sea since December 20. Dr. Volker Mohrholz
said that salt water flows throughout the water statue, which is a good sign of a large pulse. Salt
pulse means a large inflow of salt water through the Danish sisäänaits to the Baltic Sea. The salt pulse
brings deep oxygen to the Baltic Sea, which is vital to the marine ecosystem. Salt pulses are rare because
they require special conditions. This is a good time for flowing, because the surface of the Baltic Sea first
fell. Then the wind turned to the west and turned into a Pia storm that pushed fresh, salty water through the
Danish.aits. Finnish researchers also pay attention to the salt pulse. Kai Myrberg, a researcher at the Finnish
Environment Institute, wrote a messaging service in X, the Baltic Sea has a large salt pulse following a
10-year break. There is salt and oxygen in the sea. The impact of the salt pulse on the Baltic Sea is uncertain.
The pulse may remain in the southern Baltic Sea and improve its oxygen status, while pushing low oxygen
towards the north. For example, the oxygen situation in the Gulf of Finland could worsen. In 2014, a large pulse
of salt fell on the roads of Gotland. Saltwater was expected to improve the state of the depths of the Baltic Sea,
but the effects remained low. Read more about The Baltic Sea Salt Liquid Movement Amazing Scientists";

    [Fact]
    public async Task Ok()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetKeyPeopleQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetKeyPeopleQuery { Context = _context }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }
}
