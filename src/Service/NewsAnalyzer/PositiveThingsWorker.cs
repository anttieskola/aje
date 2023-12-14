﻿namespace AJE.Service.NewsAnalyzer;

public class PositiveThingsWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISender _sender;

    public PositiveThingsWorker(
        IServiceScopeFactory scopeFactory,
        ISender sender)
    {
        _scopeFactory = scopeFactory;
        _sender = sender;
    }

    private CancellationToken _cancellationToken;

    public ArticleCategory? NEWS { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;

        await ReloadAsync();

        while (!_cancellationToken.IsCancellationRequested)
        {
            await LoopAsync();
            await Task.Delay(TimeSpan.FromSeconds(3), _cancellationToken);
        }
    }

    // reloading analysis data from db to Redis
    private async Task ReloadAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();
        var rows = context.Analyses.AsAsyncEnumerable();

        await foreach (var row in rows)
        {
            if (_cancellationToken.IsCancellationRequested)
                break;

            var current = await _sender.Send(new ArticleGetByIdQuery { Id = row.Id }, _cancellationToken);
            if (current != null && current.Analysis.PositiveThingsVersion < row.PositiveThingsVersion)
            {
                await _sender.Send(new ArticleUpdatePositiveThingsCommand { Id = current.Id, PositiveThingsVersion = row.PositiveThingsVersion, PositiveThings = row.PositiveThings }, _cancellationToken);
            }
        }
    }

    // analyze loop
    private async Task LoopAsync()
    {
        var query = new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = false,
            MaxPositiveThingsVersion = ArticleGetPositiveThingsQuery.CURRENT_POSITIVE_THINGS_VERSION - 1,
            Offset = 0,
            PageSize = 1
        };
        var results = await _sender.Send(query, _cancellationToken);

        if (results.Items.Count > 0)
        {
            var article = results.Items.First();
            var positiveThings = await _sender.Send(new ArticleGetPositiveThingsQuery { Article = article }, _cancellationToken);

            var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAnalyzerContext>();

            var analysis = context.Analyses.Where(a => a.Id == article.Id).FirstOrDefault();
            if (analysis != null)
            {
                analysis.PositiveThingsVersion = ArticleGetPositiveThingsQuery.CURRENT_POSITIVE_THINGS_VERSION;
                analysis.PositiveThings = positiveThings;
            }
            else
            {
                context.Analyses.Add(new ArticleAnalysisRow
                {
                    Id = article.Id,
                    PositiveThingsVersion = ArticleGetPositiveThingsQuery.CURRENT_POSITIVE_THINGS_VERSION,
                    PositiveThings = positiveThings,
                });
            }
            await context.SaveChangesAsync(_cancellationToken);
            await _sender.Send(new ArticleUpdatePositiveThingsCommand { Id = article.Id, PositiveThingsVersion = ArticleGetPositiveThingsQuery.CURRENT_POSITIVE_THINGS_VERSION, PositiveThings = positiveThings }, _cancellationToken);
        }
    }
}
