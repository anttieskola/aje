var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        services.AddDbContext<NewsFixerContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("NewsFixer"));
        });
        services.AddDomain();
        services.AddAi(config);
        services.AddRedis(config);
        services.AddFileSystem(config);
        services.AddHostedService<ArticleTokenCalculatorWorker>();
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
