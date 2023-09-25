namespace AJE.Infra;

public class RedisConfiguratorService
{
    private IConnectionMultiplexer _connection;
    public RedisConfiguratorService(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task Initialize()
    {
        var index = new ArticleIndex(_connection);
        await index.Initialize();
    }
}
