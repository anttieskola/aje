namespace AJE.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("zeus:6379"));
        services.AddSingleton<RedisConfiguratorService>();
        return services;
    }
}
