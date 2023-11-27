using AJE.Domain.Entities;
using AJE.Domain.Exceptions;
using AJE.Service.NewsDownloader.Yle;

namespace AJE.Test.Unit.Services.News.Yle;

public class HtmlParserTests
{
    private readonly Guid _id = Guid.ParseExact("20000000-eaea-c123-4567-890120000000", "D");

    private const string _htmlNoScript = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>

    </body>
</html>
";
    [Fact]
    public void HtmlEmpty()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoScript, _id));
        Assert.Equal("unable to find last script tag", expection.Message);
    }

    private const string _htmlNoState = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
        </script>
    </body>
</html>
";
    [Fact]
    public void NoState()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoState, _id));
        Assert.Equal("unable to find initial state", expection.Message);
    }

    private const string _htmlNoArticle = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={}
        </script>
    </body>
</html>
";
    [Fact]
    public void NoArticle()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoArticle, _id));
        Assert.Equal("unable to find article", expection.Message);
    }

    private const string _htmlNoFullUrl = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoFullUrl()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoFullUrl, _id));
        Assert.Equal("no fullUrl", expection.Message);
    }

    private const string _htmlNoDate = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012""
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoDate()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoDate, _id));
        Assert.Equal("no date", expection.Message);
    }

    private const string _htmlNoTitle = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300""
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoTitle()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoTitle, _id));
        Assert.Equal("no title", expection.Message);
    }

    private const string _htmlNoLanguage = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title""
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoLanguage()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoLanguage, _id));
        Assert.Equal("no language", expection.Message);
    }

    private const string _htmlNoContent = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en""
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoContent()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoContent, _id));
        Assert.Equal("no content array", expection.Message);
    }

    private const string _htmlEmptyContentArray = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en"",
                        ""content"": []
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void EmptyContentArray()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlEmptyContentArray, _id));
        Assert.Equal("empty content array", expection.Message);
    }

    private const string _htmlOk = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en"",
                        ""content"": [
                            {
                                ""type"": ""HeadingBlock"",
                                ""level"": 1,
                                ""text"": ""Heading One""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""markdown"": ""Paragraph""
                            },
                            {
                                ""type"": ""BulletListBlock"",
                                ""items"": [
                                    ""Bullet one"",
                                    ""Bullet two""
                                ]
                            }
                        ]

                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void Ok()
    {
        var article = HtmlParser.Parse(_htmlOk, _id);
        Assert.NotNull(article);
        Assert.Equal("https://news.anttieskola.com/123456789012", article.Source);
        Assert.Equal(new DateTimeOffset(1980, 9, 12, 18, 0, 0, TimeSpan.FromHours(3)).UtcTicks, article.Modified);
        Assert.Equal("Article title", article.Title);
        Assert.Equal("en", article.Language);
        Assert.NotEmpty(article.Content);
        Assert.Equal(3, article.Content.Count());

        var headingElement = article.Content.ElementAt(0) as MarkdownHeaderElement;
        Assert.NotNull(headingElement);
        Assert.Equal(1, headingElement.Level);
        Assert.Equal("Heading One", headingElement.Text);

        var textElement = article.Content.ElementAt(1) as MarkdownTextElement;
        Assert.NotNull(textElement);
        Assert.Equal("Paragraph", textElement.Text);

        var textListElement = article.Content.ElementAt(2) as MarkdownElement;
        Assert.NotNull(textListElement);
        Assert.Equal("- Bullet one" + Environment.NewLine + "- Bullet two" + Environment.NewLine, textListElement.Text);
    }


    private const string _htmlLiveFeed = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en"",
                        ""content"": [
                            {
                                ""type"": ""LivefeedBlock"",
                                ""livefeedId"": ""64-1-1519""
                            }
                        ]

                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void LiveFeed()
    {
        var article = HtmlParser.Parse(_htmlLiveFeed, _id);
        Assert.NotNull(article);
        Assert.Equal("Article title", article.Title);
        Assert.NotEmpty(article.Content);
        Assert.Single(article.Content);

        var textElement = article.Content.ElementAt(0) as MarkdownTextElement;
        Assert.NotNull(textElement);
        Assert.Equal("64-1-1519", textElement.Text);
    }

    private const string _htmlFeatureBlockNoPages = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en"",
                        ""content"": [
                            {
                                ""type"": ""FeatureBlock""
                            }
                        ]

                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void NoPagesOnFeatureBlock()
    {
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlFeatureBlockNoPages, _id));
        Assert.Equal("no pages array in FeatureBlock", expection.Message);
    }

    private const string _htmlFeatureBlock = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""fullUrl"": ""https://news.anttieskola.com/123456789012"",
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""title"": ""Article title"",
                        ""language"": ""en"",
                        ""content"": [
                            {
                                ""type"": ""FeatureBlock"",
                                ""pages"": [
                                    {
                                        ""type"": ""header"",
                                        ""content"": [
                                            {
                                                ""type"": ""HeadingBlock"",
                                                ""text"": ""Astu 1990-luvulle""
                                            }
                                        ]
                                    },
                                    {
                                        ""type"": ""text"",
                                        ""content"": [
                                            {
                                                ""type"": ""TextBlock"",
                                                ""text"": ""C-kasettimankka oli 1990-luvulle saakka jokaisen teinin huonessa.""
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]

                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public void FeatureBlock()
    {
        var article = HtmlParser.Parse(_htmlFeatureBlock, _id);
        Assert.NotNull(article);
        Assert.Equal("Article title", article.Title);
        Assert.NotEmpty(article.Content);
        Assert.Equal(2, article.Content.Count);

        var headerElement = article.Content.ElementAt(0) as MarkdownHeaderElement;
        Assert.NotNull(headerElement);
        Assert.Equal("Astu 1990-luvulle", headerElement.Text);

        var textElement = article.Content.ElementAt(1) as MarkdownTextElement;
        Assert.NotNull(textElement);
        Assert.Equal("C-kasettimankka oli 1990-luvulle saakka jokaisen teinin huonessa.", textElement.Text);
    }
}
