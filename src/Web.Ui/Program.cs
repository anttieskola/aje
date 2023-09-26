var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfra();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IIconService, IconService>();
/*
 * Channel listing in redis after send couple messages in the chat
127.0.0.1:6379> PUBSUB CHANNELS *
 1) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:connection:ymRbGdk4qlVNInC1ehDolw"
 2) "AJEAJE.Web.Ui.Hubs.LocalChatHub:internal:groups"
 3) "AJE__Booksleeve_MasterChanged"
 4) "AJEAJE.Web.Ui.Hubs.LocalChatHub:connection:WlOXpZFQdXkTl6DA7e5IHA"
 5) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:internal:ack:GOD_05aa8bb15b0543248533058660825c90"
 6) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:all"
 7) "AJEAJE.Web.Ui.Hubs.LocalChatHub:all"
 8) "AJEAJE.Web.Ui.Hubs.LocalChatHub:connection:JiE2lIpPoGYQEJfmJcsfzg"
 9) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:internal:groups"
10) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:connection:QlfI3iIWz0TqEbdtdrG-nw"
11) "__Booksleeve_MasterChanged"
12) "AJEMicrosoft.AspNetCore.Components.Server.ComponentHub:internal:return:GOD_05aa8bb15b0543248533058660825c90"
13) "AJEAJE.Web.Ui.Hubs.LocalChatHub:internal:return:GOD_16657aa8aa40459dac1ea1c6c7d528d2"
14) "AJEAJE.Web.Ui.Hubs.LocalChatHub:connection:5qaWDhuTRLiFSVhf8ZoR-w"
15) "AJEAJE.Web.Ui.Hubs.LocalChatHub:internal:ack:GOD_16657aa8aa40459dac1ea1c6c7d528d2"

 * After shutdown of the app
127.0.0.1:6379> PUBSUB CHANNELS *
1) "__Booksleeve_MasterChanged"
 */
builder.Services.AddSignalR().AddStackExchangeRedis("zeus:6379", options =>
{
    // one redis multiple signal-r apps with separate prefixes
    options.Configuration.ChannelPrefix = "AJE";
});
builder.Logging.AddSimpleConsole(option =>
{
    option.SingleLine = true;
});
builder.Services.AddHostedService<ChatRelayService>();
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
