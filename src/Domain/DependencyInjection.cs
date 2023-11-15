namespace AJE.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IContextCreator<Article>, ArticleContextCreator>();
        services.AddSingleton<ISimplifier, MarkDownSimplifier>();
        services.AddSingleton<IPolarity, PolarityChatML>();
        services.AddSingleton<IAntai, AntaiChatML>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
