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
        services.AddDomain();
        services.AddAi(config);
        services.AddRedis(config);
        services.AddFileSystem(config);
        services.AddTranslate(config);
        services.AddHostedService<PrepperWorker>();
        // services.AddHostedService<SentimentPolarityWorker>();
        // services.AddHostedService<SummaryWorker>();
        // services.AddHostedService<PositiveThingsWorker>();
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
host.Run();
