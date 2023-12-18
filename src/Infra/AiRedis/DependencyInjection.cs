namespace AJE.Infra.AiRedis;

public static class DependencyInjection
{
    public static IServiceCollection AddAiRedis(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var llamaConfiguration = config.GetLlamaConfiguration();
        services.AddSingleton(llamaConfiguration);
        return services;
    }

    public static LlamaConfiguration GetLlamaConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(LlamaConfiguration)).Get<LlamaConfiguration>())
            ?? throw new SystemException(nameof(LlamaConfiguration));
    }
}
