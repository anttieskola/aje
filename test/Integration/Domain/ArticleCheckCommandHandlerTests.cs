using AJE.Domain;
using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Domain.Enums;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

/// <summary>
/// Tests require a llama.cpp server running in localhost:8080
/// </summary>
public class ArticleCheckCommandHandlerTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClientFixture _fixture;

    public ArticleCheckCommandHandlerTests(HttpClientFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly Article _articleYes = new()
    {
        Id = Guid.ParseExact("00000000-0000-0000-0000-000000000100", "D"),
        Category = ArticleCategory.BOGUS,
        Title = "daily test positive article",
        Modified = DateTimeOffset.UtcNow.AddYears(-100).Ticks,
        Published = true,
        Source = "https://www.anttieskola.com",
        Language = "en",
        Content = new EquatableList<MarkdownElement>
        {
            new MarkdownHeaderElement{
                Level = 1,
                Text = "Tampereen Ukraina-talolla on itsenäisyyspäivänä juhla"
            },
            new MarkdownTextElement{
                Text = "Suomen itsenäisyyspäivä on tärkeä myös ukrainalaisille. Tämän huomasi myös vapaaehtoinen Kalle Hyppölä, joka päätti järjestää Tampereen Ukraina-talolla itsenäisyyspäiväjuhlat."
            },
            new MarkdownTextElement
            {
                Text = "Juhliin aikoo osallistua myös ukrainalainen Khrystyna Lieskakova."
            },
            new MarkdownTextElement
            {
                Text = "Suomi on hyvä esimerkki Ukrainalle siitä, kuinka tärkeä on itsenäisyys. Ukrainassa ja Suomessa on samanlaisia tarinoita. Suomessa on melkein sama historia kuin Ukrainassa nyt."
            },
            new MarkdownTextElement
            {
                Text = "Lieskakova kertoo ukrainalaisen suhtautuvan itsenäisyyspäivään vielä merkittävämpänä asiana kotimaassa olevan sodan takia. Itsenäisyyttä pitää puolustaa, eikä se ole itsestäänselvyys."
            },
            new MarkdownTextElement
            {
                Text = "Suomalaiset tietävät, mitä tarkoittaa, kun itsenäisyyden hinta on valitettavasti todella korkea. Toivon, että kohta Ukraina voittaa ja koko maailma tietää, että se on itsenäinen."
            },
            new MarkdownTextElement
            {
                Text = "Lieskakova muutti tyttärensä kanssa Tampereelle lähes kaksi vuotta sitten, kun Venäjä oli hyökännyt Ukrainaan. Lieskakova on ammatiltaan englannin ja ukrainan kielen opettaja, ja hän auttaa vapaaehtoisena ukrainalaisia."
            },
            new MarkdownTextElement
            {
                Text = "Suomen itsenäisyys on minulle todella tärkeä, koska isäpuoleni on suomalainen ja äitini on asunut 16 vuotta Suomessa. Mummupuoleni on kaksi kertaa lähtenyt evakkoon Karjalasta."
            },
        },
        Polarity = Polarity.Positive,
        PolarityVersion = 1
    };
    [Fact]
    public async Task Yes()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/tmp" };
        var handler = new ArticleCheckQueryHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            new CheckArticleChatML(),
            new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory),
            new AiLogger(configuration));
        var command = new ArticleCheckQuery { Article = _articleYes };
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(response);
        Assert.True(response.IsValid);
    }

    private readonly Article _articleNo01 = new()
    {
        Id = Guid.ParseExact("00000000-0000-0000-0000-000000000100", "D"),
        Category = ArticleCategory.BOGUS,
        Title = "daily test positive article",
        Modified = DateTimeOffset.UtcNow.AddYears(-100).Ticks,
        Published = true,
        Source = "https://www.anttieskola.com",
        Language = "en",
        Content = new EquatableList<MarkdownElement>
        {
            new MarkdownTextElement{
                Text = "Ulkomailla asuvat suomalaiset kertovat Ylen haastatteluissa, mitä Suomessa ehkä jopa itsestäänselvää ei maailmalta saa."
            },
        },
        Polarity = Polarity.Positive,
        PolarityVersion = 1
    };
    [Fact]
    public async Task No01()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/tmp" };
        var handler = new ArticleCheckQueryHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            new CheckArticleChatML(),
            new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory),
            new AiLogger(configuration));
        var command = new ArticleCheckQuery { Article = _articleNo01 };
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(response);
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Reasoning);
    }

    private readonly Article _articleNo02 = new()
    {
        Id = Guid.ParseExact("00000000-0000-0000-0000-000000000100", "D"),
        Category = ArticleCategory.BOGUS,
        Title = "daily test positive article",
        Modified = DateTimeOffset.UtcNow.AddYears(-100).Ticks,
        Published = true,
        Source = "https://www.anttieskola.com",
        Language = "en",
        Content = new EquatableList<MarkdownElement>
        {
            new MarkdownTextElement{
                Text = "64-1-2135"
            },
        },
        Polarity = Polarity.Positive,
        PolarityVersion = 1
    };
    [Fact]
    public async Task No02()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/tmp" };
        var handler = new ArticleCheckQueryHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            new CheckArticleChatML(),
            new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory),
            new AiLogger(configuration));
        var command = new ArticleCheckQuery { Article = _articleNo02 };
        var response = await handler.Handle(command, CancellationToken.None);
        Assert.NotNull(response);
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Reasoning);
    }
}
