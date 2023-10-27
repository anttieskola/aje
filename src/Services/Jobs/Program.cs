var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var yleConfig = (config.GetSection(nameof(MolConfiguration)).Get<MolConfiguration>())
            ?? throw new SystemException(nameof(MolConfiguration));
        services.AddSingleton(yleConfig);

        services.AddApplication();
        services.AddDomain();
        services.AddInfra(config);
        services.AddHostedService<MolWorker>();
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
