namespace AJE.Application.Commands;

public class PublishArticleCommandHandler : IRequestHandler<PublishArticleCommand, ArticlePublishedEvent>
{
    private readonly IConnectionMultiplexer _connection;

    public PublishArticleCommandHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<ArticlePublishedEvent> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();

        var redisId = $"{ArticleConstants.INDEX_PREFIX}{request.Article.Id}";
        if (await db.KeyExistsAsync(redisId))
        {
            throw new KeyExistsException(redisId);
        }

        var json = db.JSON();
        if (!await json.SetAsync(redisId, "$", JsonSerializer.Serialize(request.Article)))
        {
            throw new KeySetException($"failed to set value {redisId}");
        }

        var publishEvent = new ArticlePublishedEvent { Id = request.Article.Id };
        await db.PublishAsync(ArticleConstants.CHANNEL, JsonSerializer.Serialize(publishEvent));
        return publishEvent;
    }
}