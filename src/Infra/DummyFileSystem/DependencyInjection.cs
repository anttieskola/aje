namespace AJE.Infra.DummyFileSystem;

public static class DependencyInjection
{
    public static IServiceCollection AddDummyFileSystem(
        this IServiceCollection services)
    {
        services.AddSingleton<IYleRepository, DummyYleRepository>();
        return services;
    }
}
