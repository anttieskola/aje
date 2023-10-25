using AJE.Domain.Data;
using AJE.Infra;
using AJE.Infra.Data;
using Microsoft.Extensions.Logging;

namespace AJE.IntegrationTests;

public class RedisFixture : IDisposable
{

    public IConnectionMultiplexer Connection { get; }
    public IDatabase Database { get; }
    public IArticleRepository ArticleRepository { get; }

    public RedisFixture()
    {
        Connection = ConnectionMultiplexer.Connect("localhost:6379");
        Database = Connection.GetDatabase();
        ArticleRepository = new ArticleRepository(new Mock<ILogger<ArticleRepository>>().Object, Connection);
        var redis = new RedisService(new Mock<ILogger<RedisService>>().Object, Connection);
        redis.Initialize().Wait();
    }

    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Connection.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}