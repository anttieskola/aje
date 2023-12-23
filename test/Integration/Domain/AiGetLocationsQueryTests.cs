using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetLocationsQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetLocationsQueryTests(
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
Shops, Posts and pharmacies are open on Christmas Holidays some services are available 24
hours a day The holidays are closed for Christmas. Most food stores serve on limited opening hours.
Many grocery stores are open in Finland over Christmas holidays at least at limited opening hours.
Some of the Prismos and K-Citymarket stores serve Christmas holiness around the clock. The majority
of K-Group food stores are open, but smaller stores are closed. Most of the S-Group stores also
serve on holidays with limited opening hours. Opening hours should be checked on the S Group and
Kesko website. All Lidls are open on Christmas Eve until December 24 at 16:00. On December 25,
all Lidll stores are closed. On December 26, the stores are generally open on Sundays. The stores
are closed for Christmas. The store opens its doors after Christmas on Wednesday, December 27.
Alcohol will serve on the morning of December 23, according to normal opening hours. Post offices
are closed for Christmas. Posti's ,s are open at 10:18 p.m. on its website, and Posti recalls that
most of the mails are located in the premises of its partners. These points serve over Christmas
according to partner companies own opening hours. Many of the university's pharmacies are open
for a limited period of time. Some pharmacies are closed on Christmas. The Helsinki T.lö University
pharmacy serves as a Christmas sacred exceptionally around the clock. Christmas traffic on railways
is at its busiest till tomorrow, according to VR. There are 12 trains on Saturday. Return traffic is
focused on the day of the day when there are also 12 additional trains. Below Christmas in Helsinki
and Helsinki and Kouvola, you can also visit Santa on Friday. In addition to Pukki, the red trubaduur
plays Christmas songs at Tampere Railway Station and trains between Seinäjoki, Tampere and Jyväskylä
on Friday, VR says. There are some exceptions to the bus schedule during the saints. It therefore
recommends checking schedules on its website or application.";

    [Fact]
    public async Task Ok()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetLocationsQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetLocationsQuery { Context = _context }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(6, response.Count); // Finland, Helsinki, Kouvola, Seinäjoki, Tampere, Jyväskylä
    }
}
