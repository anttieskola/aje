namespace AJE.Infra.Redis.Data;

public class AiStoryRepository(
    ILogger<AiStoryRepository> logger,
    IConnectionMultiplexer connection) : IAiStoryRepository
{
    private readonly AiStoryIndex _index = new();
    private readonly ILogger<AiStoryRepository> _logger = logger;
    private readonly IConnectionMultiplexer _connection = connection;

    public async Task<AiStory> AddAsync(Guid storyId)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(storyId.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);

        var story = new AiStory
        {
            StoryId = storyId,
        };
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(story));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to add AiStory with id:{redisId}");
        _logger.LogTrace("AiStory added with id:{}", story.StoryId);
        return story;
    }

    public async Task<AiStory> GetAsync(Guid storyId)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(storyId.ToString());
        if (!await db.KeyExistsAsync(redisId))
            throw new KeyNotFoundException(redisId);

        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"AiStory with id:{storyId} not found");

        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var story = JsonSerializer.Deserialize<AiStory>(json);
        return story ?? throw new DataException($"invalid value in key {redisId}");
    }

    public async Task UpdateTitleAsync(Guid storyId, string title)
    {
        var redisId = _index.RedisId(storyId.ToString());
        var result = await _connection.GetDatabase().ExecuteAsync("JSON.SET", redisId, "$.title", JsonSerializer.Serialize(title));
        if (result.ToString() != "OK")
            throw new DataException($"failed to update title for AiStory with id:{redisId}");
    }

    public async Task AddChapterAsync(Guid storyId, Guid chapterId, string title)
    {
        var redisId = _index.RedisId(storyId.ToString());
        var appendResult = await _connection.GetDatabase().ExecuteAsync("JSON.ARRAPPEND", redisId, "$.chapters", JsonSerializer.Serialize(new AiStoryChapter
        {
            ChapterId = chapterId,
            Title = title,
        }));
        var resultString = appendResult.ToString() ?? throw new DataException($"failed append chapter to AiStory with id:{redisId}");
        if (!resultString.Contains("element(s)"))
        {
            _logger.LogError("failed append chapter to AiStory with id:{}, result:{}", redisId, resultString);
            throw new DataException($"failed append chapter to AiStory with id:{redisId}");
        }
        else
        {
            var count = int.Parse(resultString.Split(' ')[0]);
            if (count == 0)
                throw new DataException($"failed chapter entry to AiStory with id:{redisId}");
        }
    }
}
