namespace AJE.Infra.Redis.Events;

public class PromptStudioEventHandler : IPromptStudioEventHandler
{
    private readonly PromptStudioIndex _index = new();

    private readonly IConnectionMultiplexer _connection;

    public PromptStudioEventHandler(
        IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task SendAsync(PromptStudioEvent promptStudioEvent)
    {
        var db = _connection.GetDatabase();
        await db.PublishAsync(_index.Channel, JsonSerializer.Serialize(promptStudioEvent));
    }
}
