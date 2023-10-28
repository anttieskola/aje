namespace AJE.Infra.Ai;

public static class DependencyInjection
{
    public static IServiceCollection AddAi(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var llamaConfiguration = config.GetLlamaConfiguration();
        services.AddSingleton(llamaConfiguration);

        services.AddHttpClient();
        services.AddSingleton<IAiModel, LlamaAiModel>();
        services.AddSingleton<IAiLogger, AiLogger>();
        return services;
    }

    public static LlamaConfiguration GetLlamaConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(LlamaConfiguration)).Get<LlamaConfiguration>())
            ?? throw new SystemException(nameof(LlamaConfiguration));
    }
}
