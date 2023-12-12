namespace AJE.Infra.DummyAi;

public static class DependencyInjection
{
    public static IServiceCollection AddDummyAi(
        this IServiceCollection services)
    {
        services.AddSingleton<IAiModel, DummyAiModel>();
        services.AddSingleton<IAiLogger, DummyAiLogger>();
        return services;
    }
}
