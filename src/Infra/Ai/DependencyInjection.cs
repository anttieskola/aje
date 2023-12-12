namespace AJE.Infra.Ai;

public static class DependencyInjection
{
    public static IServiceCollection AddAi(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var llamaConfiguration = config.GetLlamaConfiguration();
        services.AddSingleton(llamaConfiguration);
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
        services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
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
