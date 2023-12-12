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
        services.AddSingleton<ITrendRepository, TrendRepository>();
        services.AddSingleton<IPromptStudioRepository, PromptStudioRepository>();
        services.AddSingleton<IPromptStudioEventHandler, PromptStudioEventHandler>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IYleEventHandler, YleEventHandler>();
        return services;
    }

    public static RedisConfiguration GetRedisConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
            ?? throw new PlatformException(nameof(RedisConfiguration));
    }

    public static async Task InitializeRedisAsync(this IServiceProvider provider)
    {
        var redis = provider.GetService<IRedisService>()
            ?? throw new PlatformException($"{nameof(IRedisService)} not found");
        await redis.Initialize();
    }
}
