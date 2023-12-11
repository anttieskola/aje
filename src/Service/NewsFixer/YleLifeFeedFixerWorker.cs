namespace AJE.Service.NewsFixer;

public class YleLifeFeedFixerWorker : BackgroundService
{
    private readonly ILogger<YleLifeFeedFixerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public YleLifeFeedFixerWorker(
        ILogger<YleLifeFeedFixerWorker> logger,
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
            // find article to fix
            // try fix article
        }

        throw new NotImplementedException();
    }
}
