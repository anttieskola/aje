var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var redisConfiguration = (config.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>())
    ?? throw new SystemException(nameof(RedisConfiguration));
builder.Services.AddSingleton(redisConfiguration);

builder.Services.AddApplication();
builder.Services.AddInfra(redisConfiguration);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConfiguration.Host, options =>
    {
        // one redis multiple signal-r apps with separate prefixes
        options.Configuration.ChannelPrefix = "AJE_";
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
