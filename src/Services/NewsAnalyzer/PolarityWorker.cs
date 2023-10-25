

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

    private CancellationToken _cancellationToken;

    public Category? NEWS { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        while (!_cancellationToken.IsCancellationRequested)
        {
            await AnalyzeLatest();
        }
    }

    private async Task AnalyzeLatest()
    {
        // get latest article that has not been analyzed
        // or has been analyzed with older polarity version
        var query = new GetArticlesQuery
        {
            Category = Category.NEWS,
            Language = "en",
            MaxPolarityVersion = GetArticlePolarityCommand.CURRENT_POLARITY_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            // analyze article "sentiment" polarity
            var article = results.Items.First();
            var command = new GetArticlePolarityCommand { Article = article };
            var result = await _sender.Send(command, _cancellationToken);

            _logger.LogInformation($"Article {article.Id} polarity: {result.Polarity}");
        }
    }
}
