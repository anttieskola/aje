namespace AJE.Service.NewsFixer;

public class YleLifeFeedUpdateWorker : BackgroundService
{
    private readonly ILogger<YleLifeFeedUpdateWorker> _logger;
    private readonly ISender _sender;

    public YleLifeFeedUpdateWorker(
        ILogger<YleLifeFeedUpdateWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    private CancellationToken _stoppingToken;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        while (!_stoppingToken.IsCancellationRequested)
        {
            var articles = await FindArticlesToUpdate();
            foreach (var article in articles)
            {
                await UpdateArticle(article);
            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    public async Task<IReadOnlyCollection<Article>> FindArticlesToUpdate()
    {
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = true,
            Offset = 0,
            PageSize = 100,
        };
        var result = await _sender.Send(query, _stoppingToken);
        return result.Items;
    }

    private async Task UpdateArticle(Article article)
    {
        var html = await _sender.Send(new YleHttpQuery { Uri = new Uri(article.Source) }, _stoppingToken);
        var newVersion = await _sender.Send(new YleHtmlParseQuery { Html = html }, _stoppingToken);
        newVersion.Id = article.Id;
        // can't compare directly because analysis / token counts are different
        // so checking for new content or end of live event
        if (!newVersion.IsLiveNews || newVersion.Content.Count > article.Content.Count)
        {
            _logger.LogInformation("Article {article.Source} has changed", article.Source);
            await _sender.Send(new YleUpdateCommand { Uri = new Uri(article.Source), Html = html }, _stoppingToken);
            await _sender.Send(new ArticleUpdateCommand { Article = newVersion }, _stoppingToken);
        }
    }
}
