var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var yleConfig = (config.GetSection(nameof(YleConfiguration)).Get<YleConfiguration>())
            ?? throw new SystemException(nameof(YleConfiguration));
        services.AddSingleton(yleConfig);

        services.AddDomain();
        services.AddAi(config);
        services.AddRedis(config);
        services.AddFileSystem(config);
        services.AddTranslate(config);
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

await host.Services.InitializeRedisAsync();
await host.Services.InitializeFileSystemAsync();
host.Run();
