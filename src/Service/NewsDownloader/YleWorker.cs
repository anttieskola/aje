namespace AJE.Service.NewsDownloader;

public class YleWorker : BackgroundService
{
    private readonly ILogger<YleWorker> _logger;
    private readonly YleConfiguration _configuration;
    private readonly ISender _sender;
    private readonly TimeSpan _refreshDelay;
    private CancellationToken _stoppingToken;
    private const string _guidCategory = "10000000";

    public YleWorker(
        ILogger<YleWorker> logger,
        YleConfiguration configuration,
        ISender sender)
    {
        _logger = logger;
        _configuration = configuration;
        _refreshDelay = TimeSpan.FromSeconds(_configuration.RefreshDelayInSeconds);
        _sender = sender;
    }

    private readonly ConcurrentBag<string> _currentLinks = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        await Reload();

        while (!_stoppingToken.IsCancellationRequested)
        {
            // update current links
            foreach (var feed in _configuration.Feeds)
                await ScanFeed(feed);

            _logger.LogInformation("Link count {}", _currentLinks.Count);

            // handle links
            foreach (var link in _currentLinks)
            {
                await HandleLink(link);
            }

            await Task.Delay(_refreshDelay, stoppingToken);
        }
    }

    private async Task<Guid> CreateId(string source)
    {
        return await _sender.Send(new GuidGetQuery
        {
            Category = _guidCategory,
            UniqueString = source,
        }, _stoppingToken);
    }

    private async Task<Article> ParseArticle(string html)
    {
        return await _sender.Send(new YleHtmlParseQuery
        {
            Html = html,
        }, _stoppingToken);
    }

    private async Task Reload()
    {
        var files = Directory.GetFiles(_configuration.DumpFolder, "*.html");
        _logger.LogInformation("Found {} files in dump folder", files.Length);

        foreach (var file in files)
        {
            var link = $"https://yle.fi/a/{Path.GetFileNameWithoutExtension(file)}";
            if (!await _sender.Send(new ArticleExistsQuery { Source = link }, _stoppingToken))
            {
                var content = await File.ReadAllTextAsync(file, _stoppingToken);
                if (await TestHtmlParse(content))
                {
                    // add
                    var article = await ParseArticle(content);
                    article.Id = await CreateId(link);
                    await _sender.Send(new ArticleAddCommand { Article = article }, _stoppingToken);
                }
                else
                {
                    // file content is invalid, downloading again to see if we get valid article
                    content = await Request(new Uri(link));
                    if (await TestHtmlParse(content))
                    {
                        // save valid article file
                        _logger.LogInformation("Article file {} fixed with valid content", file);
                        await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateHTMLFileName(link)), content, _stoppingToken);

                        // add
                        var article = await ParseArticle(content);
                        article.Id = await CreateId(link);
                        await _sender.Send(new ArticleAddCommand { Article = article }, _stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("Article file {} could not be fixed", file);
                    }
                }
            }
        }
    }

    private async Task ScanFeed(YleFeed feed)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(feed.Url, _stoppingToken);
            var content = await response.Content.ReadAsStringAsync(_stoppingToken);
            await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateRSSFileName(feed.Url)), content, _stoppingToken);
            var links = await _sender.Send(new YleRssParseQuery { Rss = XDocument.Parse(content) });
            foreach (var link in links)
            {
                if (!_currentLinks.Contains(link))
                    _currentLinks.Add(link);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error scanning feed {} {}", feed.Name, feed.Url);
        }
    }

    private async Task HandleLink(string link)
    {
        try
        {
            if (!await _sender.Send(new ArticleExistsQuery { Source = link }, _stoppingToken))
            {
                // Note, we want to crash if any errors occur in reloading
                // Yle uses [Amazon CloudFron](https://aws.amazon.com/cloudfront/)
                // So in case of high traffic or similar we might download an CloudFront error page
                // TODO: Feature that detects the error page and works around it
                var content = await Request(new Uri(link));
                await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateHTMLFileName(link)), content, _stoppingToken);
                var article = await ParseArticle(content);
                article.Id = await CreateId(link);
                await _sender.Send(new ArticleAddCommand { Article = article }, _stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling link {}", link);
        }
    }

    public async Task<bool> TestHtmlParse(string content)
    {
        try
        {
            await ParseArticle(content);
            return true;
        }
        catch (ParsingException)
        {
            return false;
        }
    }

    public async Task<string> Request(Uri uri)
    {
        using var client = new HttpClient();
        var request = CreateRequest(uri);
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _stoppingToken);
        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        {
            using var stream = await response.Content.ReadAsStreamAsync(_stoppingToken);
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            await gzipStream.CopyToAsync(decompressedStream, _stoppingToken);
            decompressedStream.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(decompressedStream).ReadToEndAsync(_stoppingToken);
        }
        else
        {
            return await response.Content.ReadAsStringAsync(_stoppingToken);
        }
    }

    public static HttpRequestMessage CreateRequest(Uri uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        request.Headers.Add("Sec-Ch-Ua", "\"Chromium\";v=\"118\", \"Microsoft Edge\";v=\"118\", \"Not=A?Brand\";v=\"99\"");
        request.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
        request.Headers.Add("Sec-Ch-Ua-Platform", "\"Linux\"");
        request.Headers.Add("Sec-Fetch-Dest", "document");
        request.Headers.Add("Sec-Fetch-Mode", "navigate");
        request.Headers.Add("Sec-Fetch-Site", "none");
        request.Headers.Add("Sec-Fetch-User", "?1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.33");
        return request;
    }

    public static string CreateRSSFileName(Uri uri)
    {
        var sb = new StringBuilder();
        uri.ToString().Where(c => char.IsAsciiLetterOrDigit(c)).ToList().ForEach(c => sb.Append(c));
        sb.Append(".xml");
        return sb.ToString();
    }

    public static string CreateHTMLFileName(string url)
    {
        var fileName = url.Replace("https://yle.fi/a/", string.Empty);
        return $"{fileName}.html";
    }
}
