namespace AJE.Ui.PublicNews;

public class Startup
{
    private readonly IConfigurationRoot _config;
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<IAiModel, DummyAiModel>();
        services.AddSingleton<IAiLogger, DummyAiLogger>();
        services.AddSingleton<IAiChatRepository, DummyAiChatRepository>();
        services.AddSingleton<IAiChatEventHandler, DummyAiChatEventHandler>();
        services.AddApplication();
        services.AddDomain();
        services.AddRedis(_config);
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddMemoryCache();
        services.AddSignalR().AddStackExchangeRedis(_config.GetRedisConfiguration().Host, options =>
        {
            // one redis multiple signal-r apps with separate prefixes
            // not required currently but good to know
            options.Configuration.ChannelPrefix = new RedisChannel("UIPUBLICNEWS_", RedisChannel.PatternMode.Auto);
        });
        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddDebug();
            config.AddConsole();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler("/Error");

        app.UseStaticFiles();
        app.UseRouting();
        app.UsePathBase("/news");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            // endpoints.MapFallbackToPage("/_Host"); // I tried both with and without  this line,
            //endpoints.MapFallbackToPage("/news/{**path:nonfile}");

        });
    }
}