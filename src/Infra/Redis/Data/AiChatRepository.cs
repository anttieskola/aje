namespace AJE.Infra.Redis.Data;

public class AiChatRepository : IAiChatRepository
{
    private readonly AiChatIndex _index = new();
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

    public async Task<AiChat> UpdateInteractionEntryAsync(Guid chatId, AiChatInteractionEntry entry)
    {
        var chat = await GetAsync(chatId);
        var current = chat.Interactions.Single(e => e.InteractionId == entry.InteractionId);
        var index = chat.Interactions.IndexOf(current);
        if (index == -1)
            throw new KeyNotFoundException($"interaction entry with id:{entry.InteractionId} not found in AiChat with id:{chatId}");

        // JSON.ARRPOP
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(chatId.ToString());
        var result = await db.ExecuteAsync("JSON.ARRPOP", redisId, $"$.interactions[{index}]");

        // JSON.ARRINSERT (arrays new size)
        result = await db.ExecuteAsync("JSON.ARRINSERT", redisId, $"$.interactions[{index}]", JsonSerializer.Serialize(entry));

        _logger.LogTrace("Updated interaction entry with id:{} in AiChat with id:{}", entry.InteractionId, chatId);
        return await GetAsync(chatId);
    }

    public async Task<AiChat> AddInteractionEntryAsync(Guid chatId, AiChatInteractionEntry entry)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(chatId.ToString());
        var appendResult = await db.ExecuteAsync("JSON.ARRAPPEND", redisId, "$.interactions", JsonSerializer.Serialize(entry));
        var resultString = appendResult.ToString() ?? throw new DataException($"failed append interaction entry to AiChat with id:{redisId}");
        if (!resultString.Contains("element(s)"))
        {
            _logger.LogError("failed append interaction entry to AiChat with id:{}, result:{}", redisId, resultString);
            throw new DataException($"failed append interaction entry to AiChat with id:{redisId}");
        }
        else
        {
            var count = int.Parse(resultString.Split(' ')[0]);
            if (count == 0)
                throw new DataException($"failed append interaction entry to AiChat with id:{redisId}");
        }
        _logger.LogTrace("Appended interaction entry to AiChat with id:{}", chatId);
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