﻿namespace AJE.Infra.Redis.Events;

public class AiChatEventHandler : IAiChatEventHandler
{
    private readonly AiChatIndex _index = new AiChatIndex();
    private readonly IConnectionMultiplexer _connection;
    public AiChatEventHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task SendAsync(AiChatEvent aiChatEvent)
    {
        var db = _connection.GetDatabase();
        await db.PublishAsync(_index.Channel, JsonSerializer.Serialize(aiChatEvent));
    }
}
