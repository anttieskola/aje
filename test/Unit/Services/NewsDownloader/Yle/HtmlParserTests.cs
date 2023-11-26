using AJE.Domain.Entities;
using AJE.Domain.Exceptions;
using AJE.Service.NewsDownloader.Yle;

namespace AJE.Test.Unit.Services.News.Yle;

public class HtmlParserTests
{
    [Fact]
    public void CreateId()
    {
        // few samples
        Assert.Equal(new Guid("1fbf9390-2e7d-75ba-a9b1-4f8896754a0a"), HtmlParser.CreateId("https://yle.fi/a/74-20025030"));
        Assert.Equal(new Guid("330c936a-80bc-150d-0839-88ffd3895c4a"), HtmlParser.CreateId("https://yle.fi/a/74-20038161"));
        Assert.Equal(new Guid("72b10cc1-e7ee-59bc-a581-140e057a9d46"), HtmlParser.CreateId("https://yle.fi/a/74-20056936"));
        Assert.Equal(new Guid("5db98199-c8aa-de84-3cac-ae64cf01c0d7"), HtmlParser.CreateId("https://yle.fi/a/74-20053559"));
        Assert.Equal(new Guid("c29b0da0-8b2b-96c3-51dd-e47b23b57361"), HtmlParser.CreateId("https://yle.fi/a/74-20053663"));
        Assert.Equal(new Guid("54bafe43-f664-4867-b8f6-385b83a5a7a6"), HtmlParser.CreateId("https://yle.fi/a/74-20053070"));

        // test
        var url1 = "https://yle.fi/a/74-20052790";
        var id1 = HtmlParser.CreateId(url1);

        var url2 = "https://yle.fi/a/3-20052790";
        var id2 = HtmlParser.CreateId(url2);

        Assert.NotEqual(id1, id2);

        var id3 = HtmlParser.CreateId(url1);
        Assert.Equal(id1, id3);
    }

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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoScript, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoState, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoArticle, string.Empty));
        Assert.Equal("unable to find article", expection.Message);
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoDate, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoTitle, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoLanguage, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoContent, string.Empty));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlEmptyContentArray, string.Empty));
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
        var article = HtmlParser.Parse(_htmlOk, string.Empty);
        Assert.NotNull(article);
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
        var article = HtmlParser.Parse(_htmlLiveFeed, string.Empty);
        Assert.NotNull(article);
        Assert.Equal("Article title", article.Title);
        Assert.NotEmpty(article.Content);
        Assert.Single(article.Content);

        var textElement = article.Content.ElementAt(0) as MarkdownTextElement;
        Assert.NotNull(textElement);
        Assert.Equal("64-1-1519", textElement.Text);
    }
}
