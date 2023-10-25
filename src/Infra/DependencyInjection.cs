namespace AJE.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var redisConfiguration = config.GetRedisConfiguration();
        services.AddSingleton(redisConfiguration);

        var llamaConfiguration = config.GetLlamaConfiguration();
        services.AddSingleton(llamaConfiguration);

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration.Host));
        services.AddSingleton<IAiModel, LlamaAiModel>();
        services.AddSingleton<IAiLogger, AiLogger>();
        services.AddSingleton<IArticleRepository, ArticleRepository>();
        services.AddSingleton<IArticleEventHandler, ArticleEventHandler>();
        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }

    public static RedisConfiguration GetRedisConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
            ?? throw new SystemException(nameof(RedisConfiguration));
    }

    public static LlamaConfiguration GetLlamaConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(LlamaConfiguration)).Get<LlamaConfiguration>())
            ?? throw new SystemException(nameof(LlamaConfiguration));
    }

    public static async Task InitializeRedis(this IServiceProvider provider)
    {
        var redis = provider.GetService<IRedisService>()
            ?? throw new SystemException($"{nameof(IRedisService)} not found");
        await redis.Initialize();
    }
}
