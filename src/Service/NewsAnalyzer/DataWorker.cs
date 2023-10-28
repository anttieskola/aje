
namespace AJE.Service.NewsAnalyzer;

// TODO, placeholder
public class DataWorker : BackgroundService
{
    private readonly ILogger<DataWorker> _logger;
    private readonly ISender _sender;
    public DataWorker(
        ILogger<DataWorker> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
