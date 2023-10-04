using AJE.Domain.Exceptions;
using AJE.Service.News.Yle;

namespace AJE.UnitTests.Services.News.Yle;

public class HtmlParserTests
{
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoScript));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoState));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoArticle));
        Assert.Equal("unable to find article", expection.Message);
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoTitle));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoLanguage));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlNoContent));
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
        var expection = Assert.Throws<ParsingException>(() => HtmlParser.Parse(_htmlEmptyContentArray));
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
        var article = HtmlParser.Parse(_htmlOk);
        Assert.NotNull(article);
        Assert.Equal("Article title", article.Title);
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
        Assert.Equal("- Bullet one\r\n- Bullet two\r\n", textListElement.Text);
    }

}
