namespace AJE.Infra;

public class RedisConfiguratorService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<RedisConfiguratorService> _logger;

    public RedisConfiguratorService(
        IServiceProvider provider,
        ILogger<RedisConfiguratorService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task Initialize()
    {
        using var scope = _provider.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<ArticleIndex>>();
        var connection = scope.ServiceProvider.GetService<IConnectionMultiplexer>();
        if (logger == null || connection == null)
        {
            throw new Exception("Logger not found");
        }
        await new ArticleIndex(logger, connection).Initialize();
    }
}
