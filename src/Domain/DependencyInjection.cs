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
        // https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
        services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        return services;
    }
}
