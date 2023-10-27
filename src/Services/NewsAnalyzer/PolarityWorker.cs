

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

        // analyze/update articles
        while (!_cancellationToken.IsCancellationRequested)
        {
            await AnalyzeLatest();
            await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationToken);
        }

        // TODO: Listen for new articles/updates
    }

    private async Task<bool> AnalyzeLatest()
    {
        // get latest article that has not been analyzed
        // or has been analyzed with older polarity version
        var query = new GetArticlesQuery
        {
            Category = Category.NEWS,
            MaxPolarityVersion = GetArticlePolarityQuery.CURRENT_POLARITY_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);
        if (results.Items.Count > 0)
        {
            // analyze article "sentiment" polarity
            var article = results.Items.First();
            var command = new GetArticlePolarityQuery { Article = article };
            var polarityEvent = await _sender.Send(command, _cancellationToken);

            // update article with new polarity
            article.Polarity = polarityEvent.Polarity;
            article.PolarityVersion = polarityEvent.PolarityVersion;
            var updateCommand = new UpdateArticleCommand { Article = article };
            await _sender.Send(updateCommand, _cancellationToken);
            _logger.LogInformation("Updated Article {} with polarity {}", article.Source, article.Polarity);
        }
        return results.TotalCount > 1;
    }
}
