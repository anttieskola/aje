using AJE.Infra.FileSystem;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        services.AddDomain();
        services.AddRedis(config);
        services.AddAi(config);
        services.AddFileSystem(config);
        services.AddTranslate(config);
        services.AddHostedService<LlamaQueueManager>();
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
