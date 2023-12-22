using AJE.Domain.Entities;
using AJE.Infra.Translate;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

/// <summary>
/// Tests require LibreTranslate server/container running
/// </summary>
public class LibreTranslateTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClientFixture _httpClientFixture;
    public LibreTranslateTests(HttpClientFixture fixture)
    {
        _httpClientFixture = fixture;
    }

    [Fact]
    public async Task TranslateFinnishToEnglish()
    {
        var translator = new LibreTranslate(new Mock<ILogger<LibreTranslate>>().Object, TestConstants.TranslateConfiguration, _httpClientFixture.HttpClientFactory);
        var result = await translator.TranslateAsync(new TranslateRequest
        {
            SourceLanguage = "fi",
            TargetLanguage = "en",
            Text = "Tämä on integraatiotesti.",
        }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("This is an integration test.", result.TranslatedText);
    }
}
