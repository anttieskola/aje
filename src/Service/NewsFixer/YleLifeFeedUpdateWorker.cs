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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        while (!_stoppingToken.IsCancellationRequested)
        {
        }

        throw new NotImplementedException();
    }

    public async Task<Article> FindArticleToUpdate()
    {
        var query = new ArticleGetManyQuery
        {

        };
        throw new NotImplementedException();
    }
}
