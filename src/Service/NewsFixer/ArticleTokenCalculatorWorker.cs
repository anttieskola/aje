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

    private CancellationToken _stoppingToken;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        // reload true data into redis if missing
        await ReloadTokenCountsAsync();

        // check articles that have not been checked yet
        while (!_stoppingToken.IsCancellationRequested)
        {
            // load all checked article data from db
            await LoadCheckedArticles();

            var article = await FindArticleToCheck();
            if (article != null)
            {
                await ArticleCountTokensAsync(article);
                await Task.Delay(TimeSpan.FromMilliseconds(100), _stoppingToken);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(120), _stoppingToken);
            }
        }
    }

    // reloading check data where value is true
    private async Task ReloadTokenCountsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        var rows = context.Articles.Where(x => x.TokenCount >= 0).AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_stoppingToken.IsCancellationRequested)
                break;

            // update article with saved IsValidated true value if it is not set
            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _stoppingToken);
            if (current.TokenCount != row.TokenCount)
                await _sender.Send(new ArticleUpdateTokenCountCommand { Id = row.Id, TokenCount = row.TokenCount }, _stoppingToken);
        }
    }

    private List<Guid> _checkedArticles = [];

    private async Task LoadCheckedArticles()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        _checkedArticles = await context.Articles.Where(x => x.TokenCount >= 0).Select(x => x.Id).ToListAsync(_stoppingToken);
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
                MaxTokenCount = -1,
            };
            var result = await _sender.Send(query, _stoppingToken);

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
        var tokenCount = await _sender.Send(new ArticleGetTokenCountQuery { Article = article }, _stoppingToken);

        // db update
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<NewsFixerContext>();
        var current = context.Articles.Find(article.Id);
        if (current == null)
        {
            await context.Articles.AddAsync(new ArticleRow
            {
                Id = article.Id,
                TokenCount = tokenCount,
                IsValid = false,
            }, _stoppingToken);
        }
        else
        {
            current.TokenCount = tokenCount;
        }
        await context.SaveChangesAsync(_stoppingToken);
        await _sender.Send(new ArticleUpdateTokenCountCommand { Id = article.Id, TokenCount = tokenCount }, _stoppingToken);
        _logger.LogInformation("Article {} updated with TokenCount: {}", article.Id, tokenCount);
    }
}
