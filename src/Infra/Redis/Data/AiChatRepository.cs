namespace AJE.Infra.Redis.Data;

public class AiChatRepository : IAiChatRepository
{
    private readonly IRedisIndex _index = new AiChatIndex();
    private readonly ILogger<AiChatRepository> _logger;
    private readonly IConnectionMultiplexer _connection;

    public AiChatRepository(
        ILogger<AiChatRepository> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task AddAsync(AiChat aiChat)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(aiChat.Id.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);

        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(aiChat));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set value {redisId}");

        _logger.LogTrace("Added aiChat {Id}", aiChat.Id);
    }
}