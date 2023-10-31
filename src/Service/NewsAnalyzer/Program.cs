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
        services.AddHostedService<SentimentPolarityWorker>();
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
