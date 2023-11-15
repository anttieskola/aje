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

    public async Task<AiChat> AddAsync(AiChatOptions options)
    {
        var db = _connection.GetDatabase();
        var id = Guid.NewGuid();
        var redisId = _index.RedisId(id.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);

        var chat = new AiChat
        {
            Id = id,
            Timestamp = DateTimeOffset.UtcNow,
        };
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(chat));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set value {redisId}");
        _logger.LogTrace("Added aiChat {Id}", chat.Id);
        return chat;
    }

    public Task<AiChat> AddHistoryEntry(Guid id, AiChatHistoryEntry entry)
    {
        // TODO
        throw new NotImplementedException();
    }

    public async Task<AiChat> GetAsync(Guid id)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());
        if (!await db.KeyExistsAsync(redisId))
            throw new KeyNotFoundException(redisId);

        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"AiChat with id {id} not found");
        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var chat = JsonSerializer.Deserialize<AiChat>(json);
        return chat ?? throw new DataException($"invalid value in key {redisId}");
    }
}