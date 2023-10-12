IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var redisConfiguration = (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
            ?? throw new SystemException(nameof(RedisConfiguration));
        services.AddSingleton(redisConfiguration);

        services.AddHostedService<ArticleWorker>();
        services.AddApplication();
        services.AddInfra(redisConfiguration);
    })
    .ConfigureLogging(logging =>
    {
        logging.AddSimpleConsole(option =>
        {
            option.SingleLine = true;
        });
    })
    .Build();

await host.Services.InitializeRedis();
host.Run();
