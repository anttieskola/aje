namespace AJE.Infra.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var redisConfiguration = config.GetRedisConfiguration();
        services.AddSingleton(redisConfiguration);

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration.Host));
        services.AddSingleton<IArticleRepository, ArticleRepository>();
        services.AddSingleton<IArticleEventHandler, ArticleEventHandler>();
        services.AddSingleton<IAiChatRepository, AiChatRepository>();
        services.AddSingleton<IAiChatEventHandler, AiChatEventHandler>();
        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }

    public static RedisConfiguration GetRedisConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
            ?? throw new SystemException(nameof(RedisConfiguration));
    }

    public static async Task InitializeRedis(this IServiceProvider provider)
    {
        var redis = provider.GetService<IRedisService>()
            ?? throw new SystemException($"{nameof(IRedisService)} not found");
        await redis.Initialize();
    }
}
