var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var redisConfiguration = (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
            ?? throw new SystemException(nameof(RedisConfiguration));
        services.AddSingleton(redisConfiguration);

        var yleConfig = (config.GetSection(nameof(YleConfiguration)).Get<YleConfiguration>())
            ?? throw new SystemException(nameof(YleConfiguration));
        services.AddSingleton(yleConfig);

        services.AddApplication();
        services.AddInfra(redisConfiguration);
        services.AddHostedService<YleWorker>();

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
