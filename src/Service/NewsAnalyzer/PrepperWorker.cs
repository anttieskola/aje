namespace AJE.Service.NewsAnalyzer;

public class PrepperWorker : BackgroundService
{
    private readonly ISender _sender;

    public PrepperWorker(
        ISender sender)
    {
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
        while (true)
        {
            var query = new ArticleGetManyQuery
            {
                Category = ArticleCategory.NEWS,
                IsLiveNews = false,
                IsValidForAnalysis = false,
                Languages = ["en", "fi", "sv"],
                Offset = 0,
                PageSize = 1,
            };
            var result = await _sender.Send(query, _stoppingToken);

            // end of articles
            if (result.Items.Count == 0)
                return null;
            else
                return result.Items.First();
        }
    }

    private async Task PrepArticleAsync(Article article)
    {
        var preppedArticle = await _sender.Send(new ArticlePrepForAnalysisQuery { Article = article }, _stoppingToken);
        // to store all results we would need
        // titleInEnglish
        // contentInEnglish
        // persons
        // links
        // ...
        await _sender.Send(new ArticleUpdateCommand { Article = preppedArticle, }, _stoppingToken);
    }
}
