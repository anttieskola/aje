namespace AJE.Infra.Redis.Data;

public class PromptStudioRepository : IPromptStudioRepository
{
    private readonly PromptStudioIndex _index = new();
    private readonly ILogger<PromptStudioRepository> _logger;
    private readonly IConnectionMultiplexer _connection;

    public PromptStudioRepository(
        ILogger<PromptStudioRepository> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task<PromptStudioSession> AddAsync(PromptStudioOptions options)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(options.SessionId.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);
        var session = new PromptStudioSession
        {
            SessionId = options.SessionId,
        };
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(session));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to add PromptStudio session with id:{redisId}");
        _logger.LogTrace("PromptStudioSession added with id:{}", options.SessionId);
        return session;
    }

    public async Task<PromptStudioSession> AddRunAsync(Guid sessionId, PromptStudioRun run)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var appendResult = await db.ExecuteAsync("JSON.ARRAPPEND", redisId, "$.runs", JsonSerializer.Serialize(run));
        var resultString = appendResult.ToString() ?? throw new DataException($"failed append run entry to PromptStudioSession with id:{redisId}");
        if (!resultString.Contains("element(s)"))
        {
            _logger.LogError("failed append run entry to PromptStudioSession with id:{}, result:{}", redisId, resultString);
            throw new DataException($"failed append run entry to PromptStudioSession with id:{redisId}");
        }
        else
        {
            var count = int.Parse(resultString.Split(' ')[0]);
            if (count == 0)
                throw new DataException($"failed append run entry to PromptStudioSession with id:{redisId}");
        }
        _logger.LogTrace("Appended run entry to PromptStudioSession with id:{}, runId:{}", sessionId, run.RunId);
        return await GetAsync(sessionId);
    }

    public async Task<PromptStudioSession> GetAsync(Guid sessionId)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        if (!await db.KeyExistsAsync(redisId))
            throw new KeyNotFoundException(redisId);

        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"PromptStudio session with id:{sessionId} not found");
        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var chat = JsonSerializer.Deserialize<PromptStudioSession>(json);
        return chat ?? throw new DataException($"invalid value in key {redisId}");
    }
}
