using AJE.Application.Queries;

namespace AJE.Tests.Populator;

public class ArticleWorker : BackgroundService
{
    private readonly ILogger<ArticleWorker> _logger;
    private readonly ISender _sender;

    public ArticleWorker(
        ILogger<ArticleWorker> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    private TimeSpan _delay = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var generator = new ArticleGenerator("en");
                var article = generator.Generate();
                if (!await _sender.Send(new ArticleExistsQuery { Source = article.Source }, stoppingToken))
                {
                    var publishEvent = await _sender.Send(new PublishArticleCommand { Article = article }, stoppingToken);
                    _logger.LogInformation("Published article: {id}", publishEvent.Id);
                }
                else
                {
                    _logger.LogInformation("Article with source: {} already exists", article.Source);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error generating/publishing article");
            }

            await Task.Delay(_delay, stoppingToken);
        }
    }
}
