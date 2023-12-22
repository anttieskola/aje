namespace AJE.Infra.Translate;

public static class DependencyInjection
{
    public static IServiceCollection AddTranslate(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var translateConfiguration = config.GetTranslateConfiguration();
        services.AddSingleton(translateConfiguration);
        services.AddSingleton<ITranslate, LibreTranslate>();
        return services;
    }

    public static TranslateConfiguration GetTranslateConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(TranslateConfiguration)).Get<TranslateConfiguration>())
            ?? throw new SystemException(nameof(TranslateConfiguration));
    }
}
