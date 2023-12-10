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
        await UpdateModified(options.SessionId);
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
        await UpdateModified(sessionId);
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

    public async Task<PaginatedList<PromptStudioSessionHeader>> GetHeadersAsync(PromptStudioGetManySessionHeadersQuery query)
    {
        var db = _connection.GetDatabase();
        var arguments = new string[] { _index.Name, "*", "SORTBY", "modified", "DESC", "RETURN", "3", "$.sessionId", "$.title", "$.modified", "LIMIT", query.Offset.ToString(), query.PageSize.ToString() };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        // first item is total count (integer)
        var rows = (RedisResult[])result!;
        var totalCount = (long)rows[0];
        var list = new List<PromptStudioSessionHeader>();
        // then pairs of key (bulk string) and value (multibulk)
        for (long i = 1; i < rows.LongLength; i += 2)
        {
            // value is in this case defined in return statement (with labels)
            // $.sessionId, sessionId-value, $.title, title-value, $.modified, modified-value
            var data = (RedisResult[])rows[i + 1]!;
            if (data.Length == 6)
            {
                var id = (string)data[1]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var title = (string)data[3]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var modified = (long?)data[5]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                list.Add(new PromptStudioSessionHeader { SessionId = Guid.Parse(id), Title = title, Modified = modified });
            }
            else
            {
                _logger.LogError("invalid data value in key:{}", rows[i]);
                _logger.LogError("invalid data Length:{}", data.Length);
                for (var e = 0; e < data.Length; e++)
                    _logger.LogError("invalid data Index:{} Item:{}", e, data[e]);

                throw new DataException($"invalid data value in key {rows[i]}");
            }
        }
        return new PaginatedList<PromptStudioSessionHeader>(list, query.Offset, totalCount);
    }

    public async Task SaveTitleAsync(Guid sessionId, string title)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var titleJson = JsonSerializer.Serialize(title);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.title", titleJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session title with id:{redisId}");

        await UpdateModified(sessionId);
    }

    public async Task SaveTemperatureAsync(Guid sessionId, double temperature)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var temperatureJson = JsonSerializer.Serialize(temperature);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.temperature", temperatureJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session temperature with id:{redisId}");

        await UpdateModified(sessionId);
    }

    public async Task SaveNumberOfTokensEvaluatedAsync(Guid sessionId, int numberOfTokensEvaluated)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var numberOfTokensEvaluatedJson = JsonSerializer.Serialize(numberOfTokensEvaluated);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.numberOfTokensEvaluated", numberOfTokensEvaluatedJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session numberOfTokensEvaluated with id:{redisId}");

        await UpdateModified(sessionId);
    }

    public async Task SaveEntityNameAsync(Guid sessionId, string entityName)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var entityNameJson = JsonSerializer.Serialize(entityName);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.entityName", entityNameJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session entityName with id:{redisId}");

        await UpdateModified(sessionId);
    }

    public async Task SaveSystemInstructionsAsync(Guid sessionId, EquatableList<string> systemInstructions)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var systemInstructionsJson = JsonSerializer.Serialize(systemInstructions.ToArray());
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.systemInstructions", systemInstructionsJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session entityName with id:{redisId}");

        await UpdateModified(sessionId);
    }

    public async Task SaveContextAsync(Guid sessionId, string context)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var contextJson = JsonSerializer.Serialize(context);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.context", contextJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session context with id:{redisId}");

        await UpdateModified(sessionId);
    }

    private async Task UpdateModified(Guid sessionId)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(sessionId.ToString());
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.modified", DateTimeOffset.UtcNow.Ticks);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to update PromptStudio session modified with id:{redisId}");
    }

}
