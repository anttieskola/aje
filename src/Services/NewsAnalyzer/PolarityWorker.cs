namespace AJE.Service.NewsAnalyzer;

public class PolarityWorker : BackgroundService
{
    private readonly ILogger<PolarityWorker> _logger;

    private readonly ISender _sender;

    public PolarityWorker(
        ILogger<PolarityWorker> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("PolarityWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
