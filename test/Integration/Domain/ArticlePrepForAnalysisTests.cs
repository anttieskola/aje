using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Domain.Queries;
using AJE.Infra.Ai;
using AJE.Infra.Translate;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

#pragma warning disable xUnit1033
[Collection("Llama")]
public class ArticlePrepForAnalysisTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033
    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public ArticlePrepForAnalysisTests(
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

    private readonly Article _articleEn = new()
    {
        Title = "Submarine",
        Language = "en",
        Content = [
            new MarkdownTextElement
            {
                Text = "Yle meteorologist **Matti Huutonen** says that aurora activity tends to be stronger in the autumn months of September-October or else in early spring, especially around the time of the equinox."
            },
            new MarkdownTextElement
            {
                Text = "*The All Points North podcast went chasing the northern lights in Finnish Lapland. Listen to the episode via this embedded player, on* [*Yle Areena*](https://areena.yle.fi/podcastit/1-4355773) *via* [*Apple*](https://podcasts.apple.com/us/podcast/all-points-north/id1678541537) *or* [*Spotify*](https://open.spotify.com/show/11M4NJ3cfmNCo0qYiIXXU1) *or wherever you get your podcasts.*"
            },
        ]
    };

    [Fact]
    public async Task English()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var translator = new LibreTranslate(new Mock<ILogger<LibreTranslate>>().Object, TestConstants.TranslateConfiguration, _httpClientFixture.HttpClientFactory);
        var handler = new ArticlePrepForAnalysisHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            translator,
            new YlePersonGatherer(),
            new MarkDownLinkGatherer(),
            aiModel);
        var article = await handler.Handle(new ArticlePrepForAnalysisQuery { Article = _articleEn }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.True(article.IsValidForAnalysis);
        Assert.NotEmpty(article.TitleInEnglish);
        Assert.NotEmpty(article.ContentInEnglish);
        Assert.Single(article.Persons);
        Assert.Equal(3, article.Links.Count);
    }

    private readonly Article _articleFi = new()
    {
        Title = "Ilveksen metsästys poikkeusluvilla toistaiseksi jäihin Pohjanmaalla hallinto-oikeus tutkii valitukset",
        Language = "fi",
        Content = [
            new MarkdownTextElement
            {
                Text = "**Erkki Esimerkki** on kokenut metsästäjä, joka on saanut poikkeusluvan ilveksen metsästykseen. Hän on tyytyväinen, että hallinto-oikeus tutkii valitukset."
            },
            new MarkdownTextElement
            {
                Text = "Ilveksen metsästys poikkeusluvilla toistaiseksi jäihin Pohjanmaalla hallinto-oikeus tutkii valitukset"
            },
            new MarkdownTextElement
            {
                Text = "Asiasta kertoi [ensimmäisenä Ilkka-Pohjalainen verkkosivuillaan. ](https://ilkkapohjalainen.fi/uutiset/riistakeskus-my%C3%B6nsi-pyyntiluvan-ilvekselle-vaasan-hallinto-oikeus-pani-luvan-j%C3%A4%C3%A4hylle)"
            }
        ]
    };

    [Fact]
    public async Task Finnish()
    {
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
        var translator = new LibreTranslate(new Mock<ILogger<LibreTranslate>>().Object, TestConstants.TranslateConfiguration, _httpClientFixture.HttpClientFactory);
        var handler = new ArticlePrepForAnalysisHandler(
            new ArticleContextCreator(new MarkDownSimplifier()),
            translator,
            new YlePersonGatherer(),
            new MarkDownLinkGatherer(),
            aiModel);
        var article = await handler.Handle(new ArticlePrepForAnalysisQuery { Article = _articleFi }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.True(article.IsValidForAnalysis);
        Assert.NotEmpty(article.TitleInEnglish);
        Assert.NotEmpty(article.ContentInEnglish);
        Assert.Single(article.Links);
        Assert.Single(article.Persons);
    }
}
