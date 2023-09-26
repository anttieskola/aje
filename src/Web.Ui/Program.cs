var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfra();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IIconService, IconService>();
builder.Logging.AddSimpleConsole(option =>
{
    option.SingleLine = true;
});
var app = builder.Build();

var rcs = app.Services.GetService<RedisConfiguratorService>();
if (rcs != null)
{
    await rcs.Initialize();
}

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
