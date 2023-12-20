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

    private readonly ConcurrentBag<Uri> _currentLinks = [];

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

    private async Task AddArticle(Uri link, string html)
    {
        var article = await _sender.Send(new YleHtmlParseQuery { Html = html }, _stoppingToken);
        article.Id = await _sender.Send(new GuidGetQuery { Category = _guidCategory, UniqueString = link.ToString() }, _stoppingToken);
        if (_configuration.PublishOnlyEnglish)
        {
            if (article.Language == "en")
                await _sender.Send(new ArticleAddCommand { Article = article }, _stoppingToken);
        }
        else
            await _sender.Send(new ArticleAddCommand { Article = article }, _stoppingToken);
    }

    private async Task Reload()
    {
        var links = await _sender.Send(new YleListQuery { }, _stoppingToken);
        foreach (var link in links)
        {
            if (!await _sender.Send(new ArticleExistsQuery { Source = link.ToString() }, _stoppingToken))
            {
                var html = await _sender.Send(new YleGetQuery { Uri = link }, _stoppingToken);
                if (await TestHtmlParse(html))
                {
                    await AddArticle(link, html);
                }
                else
                {
                    // file content is invalid, downloading again to see if we get valid article
                    html = await _sender.Send(new YleHttpQuery { Uri = link }, _stoppingToken);
                    if (await TestHtmlParse(html))
                    {
                        // save valid article file
                        _logger.LogInformation("Article file {} fixed with valid content", link.ToString());
                        await _sender.Send(new YleAddCommand { Uri = link, Html = html }, _stoppingToken);
                        await AddArticle(link, html);
                    }
                    else
                    {
                        _logger.LogWarning("Article file {} could not be fixed", link.ToString());
                    }
                }
            }
        }
    }

    private async Task ScanFeed(YleFeed feed)
    {
        try
        {
            var content = await _sender.Send(new YleHttpQuery { Uri = feed.Url }, _stoppingToken);
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

    private async Task HandleLink(Uri link)
    {
        try
        {
            if (!await _sender.Send(new ArticleExistsQuery { Source = link.ToString() }, _stoppingToken))
            {
                var html = await _sender.Send(new YleHttpQuery { Uri = link });
                await _sender.Send(new YleAddCommand { Uri = link, Html = html }, _stoppingToken);
                await AddArticle(link, html);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling link {}", link);
        }
    }

    public async Task<bool> TestHtmlParse(string html)
    {
        try
        {
            await _sender.Send(new YleHtmlParseQuery { Html = html }, _stoppingToken);
            return true;
        }
        catch (ParsingException)
        {
            return false;
        }
    }
}
