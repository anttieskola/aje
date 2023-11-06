var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var redisConfiguration = config.GetRedisConfiguration();

// dummys
builder.Services.AddSingleton<IAiModel, DummyAiModel>();
builder.Services.AddSingleton<IAiLogger, DummyAiLogger>();
builder.Services.AddSingleton<IAiChatRepository, DummyAiChatRepository>();
builder.Services.AddSingleton<IAiChatEventHandler, DummyAiChatEventHandler>();
// real
builder.Services.AddApplication();
builder.Services.AddDomain();
builder.Services.AddRedis(config);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConfiguration.Host, options =>
    {
        // one redis multiple signal-r apps with separate prefixes
        // not required currently but good to know
        options.Configuration.ChannelPrefix = new RedisChannel("UIPUBLIC_", RedisChannel.PatternMode.Auto);
    });
builder.Logging.AddSimpleConsole(option =>
{
    option.SingleLine = true;
});
builder.Services.AddHostedService<ChatRelayService>();
var app = builder.Build();

await app.Services.InitializeRedis();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<LocalChatHub>("/hublocalchat");
app.MapFallbackToPage("/_Host");
app.Run();
