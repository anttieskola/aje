namespace AJE.Service.NewsDownloader.Yle;

public static class HtmlParser
{
    public static Article Parse(string html, string source)
    {
        var js = ParseJavaScript(html);
        var state = ParseStateString(js);
        var article = ParseJson(state);
        article.Id = CreateId(source);
        article.Source = source;
        return article;
    }

    // This seems to fail to produce the same hash when the host computer changes...
    // ares vs zeus produces different hashes
    public static Guid CreateId(string source)
    {
        var data = MD5.HashData(Encoding.UTF8.GetBytes(source));
        var guid = new Guid(data);
        return guid;
    }

    private static string ParseJavaScript(string content)
    {
        var fi = content.LastIndexOf("<script type=\"text/javascript\">");
        var li = content.LastIndexOf("</script>");
        if (fi != -1 && li != -1 && li > fi)
        {
            return content[fi..(li + 9)];
        }
        throw new ParsingException("unable to find last script tag");
    }

    private static string ParseStateString(string javascript)
    {
        var fi = javascript.IndexOf("__INITIAL__STATE__=");
        var li = javascript.IndexOf("</script>");
        if (fi != -1 && li != -1 && li > fi)
        {
            return javascript[(fi + 19)..li];
        }
        throw new ParsingException("unable to find initial state");
    }

    private static Article ParseJson(string state)
    {
        var json = JsonNode.Parse(state);
        if (json != null)
        {
            var article = json["pageData"]?["article"];
            if (article != null)
            {
                return ParseArticle(article);
            }
        }
        throw new ParsingException("unable to find article");
    }

    private static Article ParseArticle(JsonNode article)
    {
        var dateJsonModified = (article["dateJsonModified"]?.ToString()) ?? throw new ParsingException("no date");
        var titleElement = article["title"] ?? throw new ParsingException("no title");
        var languageElement = article["language"] ?? throw new ParsingException("no language");
        var contentArray = (article["content"]?.AsArray()) ?? throw new ParsingException("no content array");
        var elements = new EquatableList<MarkdownElement>();
        foreach (var c in contentArray)
        {
            if (c != null)
            {
                var type = c["type"]?.ToString();
                switch (type)
                {
                    case "HeadingBlock":
                        {
                            var level = (int?)c["level"];
                            var content = (string?)c["text"];
                            if (level != null && content != null)
                            {
                                elements.Add(new MarkdownHeaderElement
                                {
                                    Level = level.Value,
                                    Text = content
                                });
                            }
                        }
                        break;
                    case "TextBlock":
                        {
                            var content = (string?)c["markdown"];
                            if (content != null)
                            {
                                elements.Add(new MarkdownTextElement
                                {
                                    Text = content
                                });
                            }
                        }
                        break;
                    case "BulletListBlock":
                        {
                            var items = c["items"]?.AsArray();
                            if (items != null)
                            {
                                var text = new StringBuilder();
                                foreach (var item in items)
                                {
                                    text.AppendLine($"- {item}");
                                }
                                elements.Add(new MarkdownTextElement
                                {
                                    Text = text.ToString(),
                                });
                            }
                        }
                        break;
                    case "LivefeedBlock":
                        {
                            var feedId = (string?)c["livefeedId"];
                            if (feedId != null)
                            {
                                elements.Add(new MarkdownTextElement
                                {
                                    Text = feedId,
                                });
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        if (elements.Count == 0)
        {
            throw new ParsingException("empty content array");
        }
        return new Article
        {
            Category = Category.NEWS,
            Polarity = Polarity.Unknown,
            PolarityVersion = 0,
            // timestamps have no milliseconds so we need to parse it manually
            Modified = DateTimeOffset.ParseExact(dateJsonModified, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture).UtcTicks,
            Title = titleElement.ToString(),
            Language = languageElement.ToString(),
            Published = true,
            Content = elements
        };
    }
}

