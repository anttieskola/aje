var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
    builder.WebHost.UseUrls("http://localhost:5506");
else
    builder.WebHost.UseUrls("http://*:5006");

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var redisConfiguration = config.GetRedisConfiguration();

builder.Services.AddDomain();
builder.Services.AddAi(config);
builder.Services.AddFileSystem(config);
builder.Services.AddRedis(config);
builder.Services.AddTranslate(config);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        // There is a limit how much data can be sent, this affects binding
        // textarea to a string property, if lenght is more than 32k? it crashes
        // https://github.com/dotnet/aspnetcore/issues/5623
        options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
    });
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