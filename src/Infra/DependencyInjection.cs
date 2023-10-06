namespace AJE.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, RedisConfiguration redisConfiguration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration.Host));
        services.AddSingleton<RedisConfiguratorService>();
        return services;
    }
}
