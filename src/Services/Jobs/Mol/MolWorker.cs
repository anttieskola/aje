namespace AJE.Service.Jobs.Mol;

public class MolWorker : BackgroundService
{
    private const string _url = "https://tyomarkkinatori.fi/api/jobpostingfulltext/search/v1/search";
    private readonly ILogger<MolWorker> _logger;

    public MolWorker(ILogger<MolWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
