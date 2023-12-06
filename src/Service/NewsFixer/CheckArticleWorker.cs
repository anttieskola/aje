namespace AJE.Service.NewsFixer;

public class CheckArticleWorker : BackgroundService
{
    private readonly ILogger<CheckArticleWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public CheckArticleWorker(
        ILogger<CheckArticleWorker> logger,
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;

        // reload true data into redis if missing
        await ReloadTruesAsync();

        // check articles that have not been checked yet
        while (!_cancellationToken.IsCancellationRequested)
        {
            // load all checked article data from db
            await LoadCheckedArticles();

            var article = await FindArticleToCheck();
            if (article != null)
            {
                await CheckArticleASync(article);
            }

            // throttle
            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationToken);
        }
    }

    // reloading check data where value is true
    private async Task ReloadTruesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        var rows = context.Articles.Where(x => x.IsValid == true).AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // update article with saved IsValidated true value if it is not set
            var current = await _sender.Send(new GetArticleByIdQuery { Id = row.Id }, _cancellationToken);
            if (current.IsValidated == false)
                await _sender.Send(new UpdateArticleIsValidatedCommand { Id = row.Id, IsValidated = true }, _cancellationToken);
        }
    }

    private List<Guid> _checkedArticles = new();

    private async Task LoadCheckedArticles()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        _checkedArticles = await context.Articles.Select(x => x.Id).ToListAsync(_cancellationToken);
    }

    private async Task<Article?> FindArticleToCheck()
    {
        var offset = 0;
        while (true)
        {
            var query = new GetArticlesQuery
            {
                Category = ArticleCategory.NEWS,
                Offset = offset,
                PageSize = 1,
                IsValidated = false,
            };
            var result = await _sender.Send(query, _cancellationToken);

            // end of articles
            if (result.Items.Count == 0)
                return null;

            var article = result.Items.First();
            if (!_checkedArticles.Contains(article.Id))
                return article;

            // try next one
            offset++;
        }
    }

    private async Task CheckArticleASync(Article article)
    {
        // ask AI
        var result = await _sender.Send(new CheckArticleQuery { Article = article }, _cancellationToken);

        // db update
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        var entity = await context.Articles.AddAsync(new ArticleRow
        {
            Id = article.Id,
            IsValid = result.IsValid,
            Reasoning = result.Reasoning,
        }, _cancellationToken);
        await context.SaveChangesAsync(_cancellationToken);

        // redis update
        if (result.IsValid)
        {
            await _sender.Send(new UpdateArticleIsValidatedCommand { Id = article.Id, IsValidated = true }, _cancellationToken);
            _logger.LogInformation($"Article {article.Id} is valid");
        }
        else
        {
            _logger.LogInformation($"Article {article.Id} is invalid: {result.Reasoning}");
        }
    }
}
