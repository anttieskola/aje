var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddApplication();
        services.AddInfra();
        services.AddHostedService<YleWorker>();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var yleConfig = (config.GetSection(nameof(YleConfiguration)).Get<YleConfiguration>())
            ?? throw new SystemException(nameof(YleConfiguration));
        services.AddSingleton(yleConfig);

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
