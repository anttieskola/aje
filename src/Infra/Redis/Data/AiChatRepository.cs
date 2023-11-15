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
        var redisId = _index.RedisId(options.ChatId.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);

        var chat = new AiChat
        {
            ChatId = options.ChatId,
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(chat));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to add AiChat with id:{redisId}");
        _logger.LogTrace("AiChat added with id:{}", chat.ChatId);
        return chat;
    }

    public async Task<AiChat> AddHistoryEntry(Guid chatId, AiChatInteractionEntry entry)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(chatId.ToString());
        var appendResult = await db.ExecuteAsync("JSON.ARRAPPEND", redisId, "$.history", JsonSerializer.Serialize(entry));
        var resultString = appendResult.ToString() ?? throw new DataException($"failed append history entry to AiChat with id:{redisId}");
        if (!resultString.Contains("element(s)"))
        {
            _logger.LogError("failed append history entry to AiChat with id:{}, result:{}", redisId, resultString);
            throw new DataException($"failed append history entry to AiChat with id:{redisId}");
        }
        _logger.LogTrace("Appended history entry to AiChat with id:{}", chatId);
        return await GetAsync(chatId);
    }

    public async Task<AiChat> GetAsync(Guid chatId)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(chatId.ToString());
        if (!await db.KeyExistsAsync(redisId))
            throw new KeyNotFoundException(redisId);

        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"AiChat with id:{chatId} not found");
        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var chat = JsonSerializer.Deserialize<AiChat>(json);
        return chat ?? throw new DataException($"invalid value in key {redisId}");
    }
}