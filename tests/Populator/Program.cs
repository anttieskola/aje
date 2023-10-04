IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ArticleWorker>();
        services.AddApplication();
        services.AddInfra();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddSimpleConsole(option =>
        {
            option.SingleLine = true;
        });
    })
    .Build();

var rcs = host.Services.GetService<RedisConfiguratorService>();
if (rcs != null)
{
    await rcs.Initialize();
}

host.Run();
