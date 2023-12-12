
namespace AJE.Domain.Queries;

public record YleHtmlParseQuery : IRequest<Article>
{
    public required string Html { get; init; }
}

public class YleHtmlParseQueryHandler : IRequestHandler<YleHtmlParseQuery, Article>
{
    public Task<Article> Handle(YleHtmlParseQuery request, CancellationToken cancellationToken)
    {
        var js = ParseJavaScript(request.Html);
        var state = ParseStateString(js);
        var article = ParseJson(state);
        return Task.FromResult(article);
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
        var fullUrl = article["fullUrl"]?.ToString();
        if (fullUrl == null)
        {
            var url = article["url"]?.AsObject();
            fullUrl = url?["full"]?.ToString() ?? throw new ParsingException("no fullUrl");
        }
        var dateJsonModified = (article["dateJsonModified"]?.ToString()) ?? throw new ParsingException("no date");
        var titleElement = article["title"]?.ToString();
        if (titleElement == null)
        {
            var headline = article["headline"]?.AsObject();
            titleElement = headline?["full"]?.ToString() ?? throw new ParsingException("no title");
        }
        var languageElement = article["language"] ?? throw new ParsingException("no language");
        var contentArray = (article["content"]?.AsArray()) ?? throw new ParsingException("no content array");
        var live = article["live"]?.AsObject(); // only found in live news
        var isLiveNews = IsLiveNews(contentArray);
        var elements = new EquatableList<MarkdownElement>();
        if (!isLiveNews)
        {
            elements.AddRange(ParseContent(contentArray));
        }
        else
        {
            if (live == null || live.Count == 0)
            {
                throw new ParsingException("no livenode");
            }
            elements.AddRange(ParseLive(live));
            // check has live news ended yeat
            isLiveNews = ParseLiveStatus(live);
        }
        if (elements.Count == 0)
        {
            throw new ParsingException("empty content array");
        }
        return new Article
        {
            Source = fullUrl,
            Category = ArticleCategory.NEWS,
            IsLiveNews = isLiveNews,
            Polarity = Polarity.Unknown,
            PolarityVersion = 0,
            // timestamps have no milliseconds so we need to parse it manually
            Modified = DateTimeOffset.ParseExact(dateJsonModified, "yyyy-MM-ddTHH:mm:sszzz", new CultureInfo("fi-FI")).UtcTicks,
            Title = titleElement.ToString(),
            Language = languageElement.ToString(),
            Content = elements
        };
    }

    private static List<MarkdownElement> ParseContent(JsonArray contentArray)
    {
        var elements = new EquatableList<MarkdownElement>();
        foreach (var c in contentArray)
        {
            if (c != null)
            {
                var type = c["type"]?.ToString();
                switch (type)
                {
                    case "FeatureBlock":
                        {
                            var pages = c["pages"]?.AsArray();
                            if (pages != null)
                            {
                                elements.AddRange(ParseFeatureBlock(pages));
                            }
                            else
                            {
                                throw new ParsingException("no pages array in FeatureBlock");
                            }
                        }
                        break;
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
                    case "text":
                        {
                            var content = (string?)c["text"];
                            if (content != null)
                            {
                                elements.Add(new MarkdownTextElement
                                {
                                    Text = content
                                });
                            }
                            break;
                        }
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
                    default:
                        break;
                }
            }
        }
        return elements;
    }

    private static List<MarkdownElement> ParseFeatureBlock(JsonArray pages)
    {
        var elements = new List<MarkdownElement>();
        foreach (var page in pages)
        {
            if (page != null)
            {
                var type = page["type"]?.ToString();
                switch (type)
                {
                    case "header":
                        {
                            var content = (JsonArray?)page["content"];
                            if (content != null)
                            {
                                foreach (var c in content)
                                {
                                    if (c != null)
                                    {
                                        var contentType = c["type"]?.ToString();
                                        if (contentType == "HeadingBlock")
                                        {
                                            var text = (string?)c["text"];
                                            if (text != null)
                                            {
                                                elements.Add(new MarkdownHeaderElement
                                                {
                                                    Level = 1,
                                                    Text = text
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "text":
                        {
                            var content = (JsonArray?)page["content"];
                            if (content != null)
                            {
                                foreach (var c in content)
                                {
                                    if (c != null)
                                    {
                                        var contentType = c["type"]?.ToString();
                                        if (contentType == "TextBlock")
                                        {
                                            var text = (string?)c["text"];
                                            if (text != null)
                                            {
                                                elements.Add(new MarkdownTextElement
                                                {
                                                    Text = text
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
        return elements;
    }

    private static bool IsLiveNews(JsonArray contentArray)
    {
        if (contentArray.Any(c => c != null && (c["type"]?.ToString() == "LivefeedBlock" || c["type"]?.ToString() == "livefeed")))
        {
            return true;
        }
        return false;
    }

    private static bool ParseLiveStatus(JsonObject liveObject)
    {
        var value = (bool?)liveObject["livemode"];
        return value ?? throw new ParsingException("no livemode");
    }

    private static List<MarkdownElement> ParseLive(JsonObject liveObject)
    {
        var elements = new List<MarkdownElement>();
        // ongoing is rawLive{} -> live{} -> features[] -> posts[] -> content{} -> blocks[]
        if (liveObject.TryGetPropertyValue("rawLive", out JsonNode? rawLive))
        {
            if (rawLive != null && rawLive.AsObject().TryGetPropertyValue("live", out JsonNode? live))
            {
                if (live != null && live.AsObject().TryGetPropertyValue("features", out JsonNode? features))
                {
                    if (features != null && features.AsArray() != null)
                        elements.AddRange(ParseLivePosts(features.AsArray()));
                }
            }
        }
        // ended is features[] -> posts[] -> content{} -> blocks[]
        else if (liveObject.TryGetPropertyValue("features", out JsonNode? features))
        {
            if (features != null && features.AsArray() != null)
                elements.AddRange(ParseLivePosts(features.AsArray()));
        }
        return elements;
    }

    private static List<MarkdownElement> ParseLivePosts(JsonArray features)
    {
        var elements = new List<MarkdownElement>();
        foreach (var feature in features)
        {
            if (feature?.AsObject() != null && feature.AsObject().TryGetPropertyValue("posts", out JsonNode? posts))
            {
                if (posts != null && posts.AsArray() != null)
                {
                    foreach (var post in posts.AsArray())
                    {
                        if (post?.AsObject() != null && post.AsObject().TryGetPropertyValue("updatedAt", out JsonNode? updatedAt))
                        {
                            if (updatedAt != null && !string.IsNullOrEmpty(updatedAt.ToString()))
                            {
                                var dateString = updatedAt.ToString();
                                var date = DateTime.Parse(dateString, null, DateTimeStyles.AdjustToUniversal);
                                var finlandZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
                                var offSet = finlandZone.GetUtcOffset(date);
                                elements.Add(new MarkdownHeaderElement
                                {
                                    Level = 6,
                                    Text = $"{date.ToBestString()} {(offSet.Hours >= 0 ? "+" : "-")}{offSet.Hours:00}:{offSet.Minutes:00}",
                                });
                            }
                        }
                        if (post?.AsObject() != null && post.AsObject().TryGetPropertyValue("content", out JsonNode? content))
                        {

                            if (content != null && content.AsObject().TryGetPropertyValue("blocks", out JsonNode? blocks))
                            {
                                if (blocks?.AsArray() != null)
                                    elements.AddRange(ParseLiveBlocks(blocks.AsArray()));
                            }
                        }
                    }
                }
            }
        }
        return elements;
    }

    private static List<MarkdownElement> ParseLiveBlocks(JsonArray blocks)
    {
        var elements = new List<MarkdownElement>();
        foreach (var block in blocks)
        {
            if (block == null)
                continue;

            var type = (string?)block["type"];
            if (type == null)
                continue;

            switch (type)
            {
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    int level = int.Parse(type[1..]);
                    elements.Add(new MarkdownHeaderElement
                    {
                        Level = level,
                        Text = block["text"]?.ToString() ?? string.Empty, // (content can have errors)
                    });
                    break;
                case "paragraph":
                    elements.AddRange(ParseLiveParagraph(block["items"]?.AsArray()));
                    break;
                default:
                    break;
            }
        }
        return elements;
    }

    private static List<MarkdownElement> ParseLiveParagraph(JsonArray? items)
    {
        var elements = new List<MarkdownElement>();
        if (items == null)
            return elements;

        foreach (var item in items)
        {
            if (item == null)
                continue;

            var type = (string?)item["type"];
            if (type == null)
                continue;

            var bold = false;
            var styling = item["styling"]?.AsObject();
            if (styling != null)
            {
                bold = (bool?)styling["bold"] ?? false;
            }
            if (type == "text")
            {
                var text = item["text"]?.ToString() ?? string.Empty; // (content can have errors)
                elements.Add(new MarkdownTextElement
                {
                    Text = bold ? $"**{text}**" : text,
                });
            }

        }
        return elements;
    }
}