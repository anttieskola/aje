namespace AJE.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, RedisConfiguration redisConfiguration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration.Host));
        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }

    public static async Task InitializeRedis(this IServiceProvider provider)
    {
        var redis = provider.GetService<IRedisService>()
            ?? throw new SystemException($"{nameof(IRedisService)} not found");
        await redis.Initialize();
    }
}
