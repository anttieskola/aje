
namespace AJE.Infra.Redis.Events;

public class ArticleEventHandler : IArticleEventHandler
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;
    public ArticleEventHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task SendAsync(ArticleEvent articleEvent)
    {
        var db = _connection.GetDatabase();
        await db.PublishAsync(_index.Channel, JsonSerializer.Serialize(articleEvent));
    }
}
