namespace AJE.Service.NewsFixer;

public class YleLifeFeedUpdateWorker : BackgroundService
{
    private readonly ILogger<YleLifeFeedUpdateWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public YleLifeFeedUpdateWorker(
        ILogger<YleLifeFeedUpdateWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _stoppingToken;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        while (!_stoppingToken.IsCancellationRequested)
        {
            var article = await FindArticlesToUpdate();
            if (article != null)
            {
                await UpdateArticle(article);
            }
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }

        throw new NotImplementedException();
    }

    public async Task<Article?> FindArticlesToUpdate()
    {
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = true,
            Offset = 0,
            PageSize = 1,
        };
        var result = await _sender.Send(query, _stoppingToken);
        return result.Items.FirstOrDefault();
    }

    private async Task UpdateArticle(Article article)
    {
        await Task.Delay(1000);
        throw new NotImplementedException();
    }

}
