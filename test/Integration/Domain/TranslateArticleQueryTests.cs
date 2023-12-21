using AJE.Domain.Entities;
using AJE.Domain.Queries;
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
                Content = TestArticle.Content,
                Language = "en",
            },
            TargetLanguage = "fi",
        }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("fi", result.Language);
        Assert.Equal(result.Content.Count, TestArticle.Content.Count);
    }
}
