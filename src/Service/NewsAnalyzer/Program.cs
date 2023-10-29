var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        services.AddDbContext<NewsAnalyzerContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("NewsAnalyzer"));
        });
        services.AddApplication();
        services.AddDomain();
        services.AddAi(config);
        services.AddRedis(config);
        // TODO: Temporary solution to save data from Redis to database
        // services.AddHostedService<PolarityWorker>();
        services.AddHostedService<OneTimeSaveWorker>();
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
