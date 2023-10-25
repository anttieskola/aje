namespace AJE.Domain;

/// <summary>
/// TODO: Should every system component register its own selected dependencies from domain?
///       - Like if we can remove create infrastructure interface to redis and so fort
///         we can move rest of commands from application to domain. Then application is left
///         without any implementation...
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IContextCreator<Article>, ArticleContextCreator>();
        services.AddSingleton<ISimplifier, MarkDownSimplifier>();
        services.AddSingleton<IPolarity, PolarityChatML>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
