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

    private async Task ScanFeed(YleFeed feed, CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(feed.Url, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateFileNameFromUrl(feed.Url, "xml")), content, ct);
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
                await File.WriteAllTextAsync(Path.Combine(_configuration.DumpFolder, CreateFileNameFromUrl(link, "html")), content, ct);
                
                var article = HtmlParser.Parse(content);
                article.Id = Guid.NewGuid();
                article.Category = ArticleCategory.NEWS;
                article.Modified = DateTime.UtcNow.Ticks;
                article.Published = true;
                article.Source = link;
                await _sender.Send(new PublishArticleCommand { Article = article }, ct);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling link {}", link);
        }
    }

    public static string CreateFileNameFromUrl(Uri uri, string extension)
        => CreateFileNameFromUrl(uri.ToString(), extension);

    public static string CreateFileNameFromUrl(string url, string extension)
    {
        var sb = new StringBuilder();
        foreach (var c in url)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                sb.Append(c);
            }
        }
        sb.Append('.');
        sb.Append(extension);
        return sb.ToString();
    }
}
