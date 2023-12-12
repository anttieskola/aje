namespace AJE.Ui.Public.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}

public class LocalChatHub : Hub<IChatClient>
{
    private readonly ILogger<LocalChatHub> _logger;

    public LocalChatHub(
        ILogger<LocalChatHub> logger)
    {
        _logger = logger;
    }

    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation("SendMessage {user} {message}", user, message);
        await Clients.All.ReceiveMessage(user, message);
    }

    public async Task SendMessageToCaller(string user, string message)
    {
        _logger.LogInformation("SendMessageToCaller {user} {message}", user, message);
        await Clients.Caller.ReceiveMessage(user, message);
    }

    public async Task SendMessageToGroup(string user, string message)
    {
        _logger.LogInformation("SendMessageToGroup {user} {message}", user, message);
        await Clients.Group("GroupName").ReceiveMessage(user, message);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected: {}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Disconnected: {}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}