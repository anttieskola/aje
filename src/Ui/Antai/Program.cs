var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
    builder.WebHost.UseUrls("http://localhost:5503");
else
    builder.WebHost.UseUrls("http://*:5003");

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var redisConfiguration = config.GetRedisConfiguration();

builder.Services.AddDummyFileSystem();

builder.Services.AddDomain();
builder.Services.AddAi(config);
builder.Services.AddRedis(config);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConfiguration.Host, options =>
    {
        // one redis multiple signal-r apps with separate prefixes
        // not required currently but good to know
        options.Configuration.ChannelPrefix = new RedisChannel("UIANTAI_", RedisChannel.PatternMode.Auto);
    });
builder.Logging.AddSimpleConsole(option =>
{
    option.SingleLine = true;
});
var app = builder.Build();

await app.Services.InitializeRedisAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();