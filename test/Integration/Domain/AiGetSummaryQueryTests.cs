using AJE.Domain.Queries;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class AiGetSummaryQueryTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public AiGetSummaryQueryTests(
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
Ahtisaari began his diplomatic career in 1973 when he became Finland's Ambassador to Tanzania, Zambia, Somalia and
Mozambique, an office he held until 1977. This new mission allowed him to get closer to East African
affairs, monitoring from Dar es Salaam the independence process of Namibia and maintaining close contacts with
South West Africa People's Organisation (SWAPO). In 1977 he was recalled by the United Nations to succeed Seán
MacBride as United Nations Commissioner for Namibia, a post he held until 1981, and as representative of Secretary-General
Kurt Waldheim from 1978.
Following the death of a later UN Commissioner for Namibia, Bernt Carlsson, on Pan Am Flight 103 on 21 December 1988
on the eve of the signing of the Tripartite Accord at UN Headquarters Ahtisaari was sent to Namibia in April 1989
as the UN Special Representative to head the United Nations Transition Assistance Group (UNTAG). Because of the illegal
incursion of SWAPO troops from Angola, the South African appointed Administrator-General (AG), Louis Pienaar, sought
Ahtisaari's agreement to the deployment of SADF troops to stabilize the situation. Ahtisaari took advice from British
prime minister Margaret Thatcher, who was visiting the region at the time, and approved the SADF deployment. A period
of intense fighting ensued when at least 375 SWAPO insurgents were killed. In July 1989, Glenys Kinnock and Tessa Blackstone
of the British Council of Churches visited Namibia and reported: ""There is a widespread feeling that too many concessions
were made to South African personnel and preferences and that Martti Ahtisaari was not forceful enough in his dealings with
the South Africans.""

Perhaps because of his reluctance to authorise this SADF deployment, Ahtisaari was alleged to have been targeted by the
South African Civil Cooperation Bureau (CCB). According to a hearing in September 2000 of the South African Truth and
Reconciliation Commission, two CCB operatives (Kobus le Roux and Ferdinand Barnard) were tasked not to kill Ahtisaari,
but to give him ""a good hiding"". To carry out the assault, Barnard had planned to use the grip handle of a metal saw
as a knuckleduster. In the event, Ahtisaari did not attend the meeting at the Keetmanshoop Hotel, where Le Roux and Barnard
lay in wait for him, and thus Ahtisaari escaped injury.

After the independence elections of 1989, Ahtisaari and his wife were made honorary Namibian citizens in 1992. South Africa
gave him the O R Tambo award for ""his outstanding achievement as a diplomat and commitment to the cause of freedom in Africa
and peace in the world"".

Ahtisaari served as UN undersecretary-general for administration and management from 1987 to 1991 causing mixed feelings
inside the organisation during an internal investigation of massive fraud. When Ahtisaari revealed in 1990 that he had
secretly lengthened the grace period allowing UN officials to return misappropriated taxpayer money from the original
three months to three years, the investigators were furious. The 340 officials found guilty of fraud were able to return
money even after their crime had been proven. The harshest punishment was the firing of twenty corrupt officials
    ";

    [Fact]
    public async Task Ok()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var handler = new AiGetSummaryQueryHandler(aiModel);
        var response = await handler.Handle(new AiGetSummaryQuery { Context = _context }, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.True(response.Length < _context.Length);
    }
}
