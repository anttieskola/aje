namespace AJE.Service.NewsAnalyzer;

/// <summary>
///
/// </summary>
public class AnalysisPrepperWorker : BackgroundService
{
    private readonly ILogger<AnalysisPrepperWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public AnalysisPrepperWorker(
        ILogger<AnalysisPrepperWorker> logger,
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

        // check articles that have not been checked yet
        while (!_stoppingToken.IsCancellationRequested)
        {
            var article = await FindArticleToPrep();
            if (article != null)
            {
                await PrepArticleAsync(article);
                await Task.Delay(TimeSpan.FromMilliseconds(100), _stoppingToken);
            }
            else
            {
                await Task.Delay(TimeSpan.FromMinutes(2), _stoppingToken);
            }
        }

    }

    private async Task<Article?> FindArticleToPrep()
    {
        throw new NotImplementedException();
        var offset = 0;
        while (true)
        {
            var query = new ArticleGetManyQuery
            {
                Category = ArticleCategory.NEWS,
                IsLiveNews = false,
                IsValidForAnalysis = false,
                Offset = offset,
                PageSize = 1,
                MaxTokenCount = -1,
            };
            var result = await _sender.Send(query, _stoppingToken);

            // end of articles
            if (result.Items.Count == 0)
                return null;


            // try next one
            offset++;
        }
    }

    private async Task PrepArticleAsync(Article article)
    {
        await Task.Delay(10000);
    }
}
