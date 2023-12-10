namespace AJE.Service.NewsFixer;

public class ArticleTokenCalculatorWorker : BackgroundService
{
    private readonly ILogger<ArticleTokenCalculatorWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public ArticleTokenCalculatorWorker(
        ILogger<ArticleTokenCalculatorWorker> logger,
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
        await ReloadTokenCountsAsync();

        // check articles that have not been checked yet
        while (!_cancellationToken.IsCancellationRequested)
        {
            // load all checked article data from db
            await LoadCheckedArticles();

            var article = await FindArticleToCheck();
            if (article != null)
            {
                await ArticleCountTokensAsync(article);
            }

            // throttle
            await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationToken);
        }
    }

    // reloading check data where value is true
    private async Task ReloadTokenCountsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        var rows = context.Articles.AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            // update article with saved IsValidated true value if it is not set
            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (!current.IsValidated)
                await _sender.Send(new ArticleUpdateIsValidatedCommand { Id = row.Id, IsValidated = true }, _cancellationToken);
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
            var query = new ArticleGetManyQuery
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

    private async Task ArticleCountTokensAsync(Article article)
    {
        // ask AI
        var tokenCount = await _sender.Send(new ArticleGetTokenCountQuery { Article = article }, _cancellationToken);

        // db update
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        await context.Articles.AddAsync(new ArticleRow
        {
            Id = article.Id,
            TokenCount = tokenCount,
            IsValid = false,
        }, _cancellationToken);
        await context.SaveChangesAsync(_cancellationToken);
        await _sender.Send(new ArticleUpdateTokenCountCommand { Id = article.Id, TokenCount = tokenCount }, _cancellationToken);
        _logger.LogInformation("Article {} updated with TokenCount: {}", article.Id, tokenCount);
    }
}
