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

        services.AddApplication();
        services.AddDomain();
        services.AddRedis(config);
        services.AddHostedService<YleWorker>();

        // dummys
        services.AddSingleton<IAiModel, DummyAiModel>();
        services.AddSingleton<IAiLogger, DummyAiLogger>();
        services.AddSingleton<IAiChatRepository, DummyAiChatRepository>();
        services.AddSingleton<IAiChatEventHandler, DummyAiChatEventHandler>();
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
