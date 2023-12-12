using AJE.Domain.Entities;
using AJE.Domain.Exceptions;
using AJE.Domain.Queries;

namespace AJE.Test.Unit.Domain.Queries;

public class YleHtmlParseQueryHandlerTests
{
    #region missing data
    private const string _htmlNoScript = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>

    </body>
</html>
";
    [Fact]
    public async Task HtmlEmpty()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoScript }, CancellationToken.None));
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
    public async Task NoState()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoState }, CancellationToken.None));
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
    public async Task NoArticle()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoArticle }, CancellationToken.None));
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
    public async Task NoFullUrl()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoFullUrl }, CancellationToken.None));
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
    public async Task NoDate()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoDate }, CancellationToken.None));
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
    public async Task NoTitle()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoTitle }, CancellationToken.None));
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
    public async Task NoLanguage()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoLanguage }, CancellationToken.None));
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
    public async Task NoContent()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlNoContent }, CancellationToken.None));
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
    public async Task EmptyContentArray()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlEmptyContentArray }, CancellationToken.None));
        Assert.Equal("empty content array", expection.Message);
    }
    #endregion missing data

    #region normal news

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
    public async Task Ok()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlOk }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.Equal("https://news.anttieskola.com/123456789012", article.Source);
        Assert.Equal(new DateTimeOffset(1980, 9, 12, 18, 0, 0, TimeSpan.FromHours(3)).UtcTicks, article.Modified);
        Assert.Equal("Article title", article.Title);
        Assert.Equal("en", article.Language);
        Assert.NotEmpty(article.Content);
        Assert.Equal(3, article.Content.Count);

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

    private const string _htmlAlternativeFullUrlOk = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""url"": {
                            ""full"": ""https://news.anttieskola.com/123456789012""
                        },
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
    public async Task AlterFullUrl()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlAlternativeFullUrlOk }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.Equal("https://news.anttieskola.com/123456789012", article.Source);
        Assert.Equal(new DateTimeOffset(1980, 9, 12, 18, 0, 0, TimeSpan.FromHours(3)).UtcTicks, article.Modified);
        Assert.Equal("Article title", article.Title);
        Assert.Equal("en", article.Language);
        Assert.NotEmpty(article.Content);
        Assert.Equal(3, article.Content.Count);

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

    private const string _htmlAlternativeHeadlineOk = @"
<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <script type=""text/javascript"">
            window.__INITIAL__STATE__={
                ""pageData"": {
                    ""article"": {
                        ""url"": {
                            ""full"": ""https://news.anttieskola.com/123456789012""
                        },
                        ""dateJsonModified"": ""1980-09-12T18:00:00+0300"",
                        ""headline"": {
                            ""full"": ""Article title""
                        },
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
    public async Task AlterHeadline()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlAlternativeHeadlineOk }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.Equal("https://news.anttieskola.com/123456789012", article.Source);
        Assert.Equal(new DateTimeOffset(1980, 9, 12, 18, 0, 0, TimeSpan.FromHours(3)).UtcTicks, article.Modified);
        Assert.Equal("Article title", article.Title);
        Assert.Equal("en", article.Language);
        Assert.NotEmpty(article.Content);
        Assert.Equal(3, article.Content.Count);

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

    #endregion normal news

    #region feature block
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
    public async void NoPagesOnFeatureBlock()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlFeatureBlockNoPages }, CancellationToken.None));
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
    public async void FeatureBlock()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlFeatureBlock }, CancellationToken.None);
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
    #endregion feature block

    #region live feed

    private const string _htmlLiveFeedNoLiveNode = @"
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
    public async Task LiveFeedNoLiveNode()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlLiveFeedNoLiveNode }, CancellationToken.None));
        Assert.Equal("no livenode", expection.Message);
    }

    private const string _htmlLiveFeedNoLiveMode = @"
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
                        ],
                        ""live"": {
                            ""somethingForTheCount"": false
                        }
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public async Task LiveFeedNoLiveMode()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlLiveFeedNoLiveMode }, CancellationToken.None));
        Assert.Equal("no livemode", expection.Message);
    }

    private const string _htmlLiveFeedEmptyContent = @"
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
                        ],
                        ""live"": {
                            ""livemode"": true
                        }
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public async Task LiveFeedNoContent()
    {
        var handler = new YleHtmlParseQueryHandler();
        var expection = await Assert.ThrowsAsync<ParsingException>(async () => await handler.Handle(new YleHtmlParseQuery { Html = _htmlLiveFeedEmptyContent }, CancellationToken.None));
        Assert.Equal("empty content array", expection.Message);
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
                        ],
                        ""live"": {
                            ""livemode"": true,
                            ""rawLive"": {
                                ""live"": {
                                    ""features"": [
                                        {
                                            ""posts"": [
                                                {
                                                    ""updatedAt"": ""2100-12-24T12:00:00.000000Z"",
                                                    ""content"": {
                                                        ""blocks"": [
                                                            {
                                                                ""type"": ""h1"",
                                                                ""text"": ""Ufot ovat laskeutuneet""
                                                            },
                                                            {
                                                                ""type"": ""paragraph"",
                                                                ""items"": [
                                                                    {
                                                                        ""type"": ""text"",
                                                                        ""text"": ""He valitsivat laskeutumispaikaksi Oulun kaupungin.""
                                                                    },
                                                                    {
                                                                        ""type"": ""text"",
                                                                        ""styling"": {
                                                                            ""bold"": true
                                                                        },
                                                                        ""text"": ""Miksi Oulu?""
                                                                    }
                                                                ]
                                                            }
                                                        ]
                                                    }
                                                },
                                                {
                                                    ""updatedAt"": ""2100-12-24T13:00:00.000000Z"",
                                                    ""content"": {
                                                        ""blocks"": [
                                                            {
                                                                ""type"": ""h1"",
                                                                ""text"": ""Ufot ovat paenneet""
                                                            },
                                                            {
                                                                ""type"": ""paragraph"",
                                                                ""items"": [
                                                                    {
                                                                        ""type"": ""text"",
                                                                        ""text"": ""Alus lähti aikaisin tänään, kommenttina kuului vain, että""
                                                                    },
                                                                    {
                                                                        ""type"": ""text"",
                                                                        ""styling"": {
                                                                            ""bold"": true
                                                                        },
                                                                        ""text"": ""Ihan liian kova meininki!""
                                                                    }
                                                                ]
                                                            }
                                                        ]
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                }
                            }
                        }
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public async Task LiveFeed()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlLiveFeed }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.True(article.IsLiveNews);
        Assert.Equal(8, article.Content.Count);
        Assert.Equal("2100-12-24 12:00:00 +02:00", (article.Content.ElementAt(0) as MarkdownHeaderElement)?.Text);
        Assert.Equal("Ufot ovat laskeutuneet", (article.Content.ElementAt(1) as MarkdownHeaderElement)?.Text);
        Assert.Equal("He valitsivat laskeutumispaikaksi Oulun kaupungin.", (article.Content.ElementAt(2) as MarkdownTextElement)?.Text);
        Assert.Equal("**Miksi Oulu?**", (article.Content.ElementAt(3) as MarkdownTextElement)?.Text);
        Assert.Equal("2100-12-24 12:00:00 +02:00", (article.Content.ElementAt(0) as MarkdownHeaderElement)?.Text);
        Assert.Equal("Ufot ovat paenneet", (article.Content.ElementAt(5) as MarkdownHeaderElement)?.Text);
        Assert.Equal("Alus lähti aikaisin tänään, kommenttina kuului vain, että", (article.Content.ElementAt(6) as MarkdownTextElement)?.Text);
        Assert.Equal("**Ihan liian kova meininki!**", (article.Content.ElementAt(7) as MarkdownTextElement)?.Text);
    }

    private const string _htmlLiveFeedEnded = @"
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
                        ],
                        ""live"": {
                            ""livemode"": false,
                            ""features"": [
                                {
                                    ""posts"": [
                                        {
                                            ""updatedAt"": ""1980-09-12T12:00:00.000000Z"",
                                            ""content"": {
                                                ""blocks"": [
                                                    {
                                                        ""type"": ""h1"",
                                                        ""text"": ""Israel on antanut palestiinalaisille oman maan ja itsenäisyyden""
                                                    },
                                                    {
                                                        ""type"": ""paragraph"",
                                                        ""items"": [
                                                            {
                                                                ""type"": ""text"",
                                                                ""text"": ""Netanjahu on iloinnut kun saanut luovuttaa maata hyvään käyttöön""
                                                            },
                                                            {
                                                                ""type"": ""text"",
                                                                ""text"": ""Kertoi hän haastattelussa YK:n kokouksessa""
                                                            },
                                                            {
                                                                ""type"": ""text"",
                                                                ""text"": ""Hän kehui alieneja hienosta rauhanvälitystyöstä""
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        ""type"": ""h2"",
                                                        ""text"": ""I could not ask for better day than this""
                                                    },
                                                    {
                                                        ""type"": ""paragraph"",
                                                        ""items"": [
                                                            {
                                                                ""type"": ""text"",
                                                                ""text"": ""Lauloivat alienit Elon Muskin bändin säestäessä""
                                                            },
                                                            {
                                                                ""type"": ""text"",
                                                                ""text"": ""Elvis laskeutuu avaruusaluksesta soittamaan kitaraa ryhmän mukaan""
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    }
                }
            }
        </script>
    </body>
</html>
";
    [Fact]
    public async Task LiveFeedEnded()
    {
        var handler = new YleHtmlParseQueryHandler();
        var article = await handler.Handle(new YleHtmlParseQuery { Html = _htmlLiveFeedEnded }, CancellationToken.None);
        Assert.NotNull(article);
        Assert.False(article.IsLiveNews);
        Assert.Equal(8, article.Content.Count);
        Assert.Equal("1980-09-12 12:00:00 +03:00", (article.Content.ElementAt(0) as MarkdownHeaderElement)?.Text);
        Assert.Equal(6, (article.Content.ElementAt(0) as MarkdownHeaderElement)?.Level);
        Assert.Equal("Israel on antanut palestiinalaisille oman maan ja itsenäisyyden", (article.Content.ElementAt(1) as MarkdownHeaderElement)?.Text);
        Assert.Equal("Netanjahu on iloinnut kun saanut luovuttaa maata hyvään käyttöön", (article.Content.ElementAt(2) as MarkdownTextElement)?.Text);
        Assert.Equal("Kertoi hän haastattelussa YK:n kokouksessa", (article.Content.ElementAt(3) as MarkdownTextElement)?.Text);
        Assert.Equal("Hän kehui alieneja hienosta rauhanvälitystyöstä", (article.Content.ElementAt(4) as MarkdownTextElement)?.Text);
        Assert.Equal("I could not ask for better day than this", (article.Content.ElementAt(5) as MarkdownHeaderElement)?.Text);
        Assert.Equal(2, (article.Content.ElementAt(5) as MarkdownHeaderElement)?.Level);
        Assert.Equal("Lauloivat alienit Elon Muskin bändin säestäessä", (article.Content.ElementAt(6) as MarkdownTextElement)?.Text);
        Assert.Equal("Elvis laskeutuu avaruusaluksesta soittamaan kitaraa ryhmän mukaan", (article.Content.ElementAt(7) as MarkdownTextElement)?.Text);
    }
    #endregion live feed
}
