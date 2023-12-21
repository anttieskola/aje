using AJE.Domain.Entities;
using AJE.Domain.Queries;
using AJE.Infra.FileSystem;
using AJE.Infra.Translate;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

/// <summary>
/// Tests require LibreTranslate server/container running
/// </summary>
public class TranslateArticleQueryTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClientFixture _httpClientFixture;

    public TranslateArticleQueryTests(HttpClientFixture fixture)
    {
        _httpClientFixture = fixture;
    }

    [Fact]
    public async Task TestTranslateEnglishToFinnish()
    {
        var translator = new LibreTranslate(new Mock<ILogger<LibreTranslate>>().Object, TestConstants.TranslateConfiguration, _httpClientFixture.HttpClientFactory);
        var handler = new TranslateArticleQueryHandler(translator);
        var result = await handler.Handle(new TranslateArticleQuery
        {
            Article = new Article
            {
                Title = "Submarine",
                Content = TestArticle.Content,
                Language = "en",
            },
            TargetLanguage = "fi",
        }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("fi", result.Language);
        Assert.Equal("Sukellusvene", result.Title);
        Assert.Equal(result.Content.Count, TestArticle.Content.Count);
    }

    [Fact]
    public async Task TestRealArticle()
    {
        var yleRepository = new YleRepository(TestConstants.FileSystemConfiguration);
        var html = await yleRepository.GetHtmlAsync(new Uri("https://yle.fi/a/74-20056978"), CancellationToken.None);
        var htmlParserHandler = new YleHtmlParseQueryHandler();
        var article = await htmlParserHandler.Handle(new YleHtmlParseQuery { Html = html }, CancellationToken.None);

        var translator = new LibreTranslate(new Mock<ILogger<LibreTranslate>>().Object, TestConstants.TranslateConfiguration, _httpClientFixture.HttpClientFactory);
        var handler = new TranslateArticleQueryHandler(translator);

        var result = await handler.Handle(new TranslateArticleQuery { Article = article }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(23, result.Content.Count);
    }
}
