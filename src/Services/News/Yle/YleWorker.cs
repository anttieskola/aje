namespace AJE.Service.News.Yle;

public class YleWorker : BackgroundService
{
    private readonly ILogger<YleWorker> _logger;
    private readonly YleConfiguration _configuration;
    private readonly ISender _sender;
    private readonly TimeSpan _refreshDelay;

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

    private readonly ConcurrentBag<string> _currentLinks = new();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Reload(ct);

        while (!ct.IsCancellationRequested)
        {
            // update current links
            foreach (var feed in _configuration.Feeds)
                await ScanFeed(feed, ct);

            _logger.LogInformation("Link count {}", _currentLinks.Count);

            // handle links
            foreach (var link in _currentLinks)
            {
                await HandleLink(link, ct);
            }

            await Task.Delay(_refreshDelay, ct);
        }
    }

    private async Task Reload(CancellationToken ct)
    {
        // Note, we want to crash if any errors occur in reloading

        var files = Directory.GetFiles(_configuration.DumpFolder, "*.html");
        _logger.LogInformation("Found {} files in dump folder", files.Length);

        foreach (var file in files)
        {
            var source = $"https://yle.fi/a/{Path.GetFileNameWithoutExtension(file)}";
            if (!await _sender.Send(new ArticleExistsQuery { Source = source }, ct))
            {
                var content = await File.ReadAllTextAsync(file, ct);
                var article = HtmlParser.Parse(content);
                article.Source = source;
                await _sender.Send(new PublishArticleCommand { Article = article }, ct);
            }
        }
    }

    private async Task ScanFeed(YleFeed feed, CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(feed.Url, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateRSSFileName(feed.Url)), content, ct);
            var links = RssParser.Parse(content);
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


    private async Task HandleLink(string link, CancellationToken ct)
    {
        try
        {
            if (!await _sender.Send(new ArticleExistsQuery { Source = link }, ct))
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(link, ct);
                var content = await response.Content.ReadAsStringAsync(ct);
                await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateHTMLFileName(link)), content, ct);

                var article = HtmlParser.Parse(content);
                article.Source = link;
                await _sender.Send(new PublishArticleCommand { Article = article }, ct);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling link {}", link);
        }
    }

    public static string CreateRSSFileName(Uri uri)
    {
        var sb = new StringBuilder();
        foreach (var c in uri.ToString())
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                sb.Append(c);
            }
        }
        sb.Append(".xml");
        return sb.ToString();
    }

    public static string CreateHTMLFileName(string url)
    {
        var fileName = url.Replace("https://yle.fi/a/", string.Empty);
        return $"{fileName}.html";
    }
}
