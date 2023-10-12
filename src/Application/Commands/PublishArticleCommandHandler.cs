namespace AJE.Application.Commands;

public class PublishArticleCommandHandler : IRequestHandler<PublishArticleCommand, ArticlePublishedEvent>
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;

    public PublishArticleCommandHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<ArticlePublishedEvent> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(request.Article.Id.ToString());
        if (await db.KeyExistsAsync(redisId))
        {
            throw new KeyExistsException(redisId);
        }
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(request.Article));
        if (setResult.ToString() != "OK")
        {
            throw new DataException($"failed to set value {redisId}");
        }
        var publishEvent = new ArticlePublishedEvent { Id = request.Article.Id };
        await db.PublishAsync(_index.Channel, JsonSerializer.Serialize(publishEvent));
        return publishEvent;
    }
}