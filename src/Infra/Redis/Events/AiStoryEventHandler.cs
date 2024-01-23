
namespace AJE.Infra.Redis.Events;

public class AiStoryEventHandler : IAiStoryEventHandler
{
    private readonly AiStoryIndex _index = new();
    private readonly IConnectionMultiplexer _connection;
    public AiStoryEventHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task SendAsync(AiStoryEvent aiStoryEvent)
    {
        await _connection.GetDatabase().PublishAsync(_index.Channel, JsonSerializer.Serialize(aiStoryEvent));
    }
}
