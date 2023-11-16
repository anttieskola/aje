namespace AJE.Infra.Redis;

public interface IRedisService
{
    Task Initialize();
}

public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> _logger;
    private readonly IConnectionMultiplexer _connection;

    public RedisService(
        ILogger<RedisService> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    private readonly IEnumerable<IRedisIndex> _indexes = new List<IRedisIndex>
    {
         new ArticleIndex(),
         new AiChatIndex(),
    };

    public async Task Initialize()
    {
        var db = _connection.GetDatabase();
        var indexes = await GetIndexes(db);

        foreach (var index in _indexes)
        {
            if (indexes.Contains(index.Name))
            {
                var indexVersion = await db.StringGetAsync($"{index.Name}_VERSION");
                if (!indexVersion.HasValue)
                {
                    _logger.LogInformation("Index {} has no version, deleting and recreating", index.Name);
                    await DeleteIndex(db, index);
                }
                else
                {
                    if (int.TryParse(indexVersion, out int version))
                    {
                        if (version == index.Version)
                            continue;
                        else
                        {
                            await DeleteIndex(db, index);
                            _logger.LogInformation("Index {} has version {}, deleting and recreating", index.Name, version);
                        }
                    }
                    else
                    {
                        await DeleteIndex(db, index);
                        _logger.LogInformation("Index {} version can't be parsed, deleting and recreating", index.Name);
                    }
                }

            }
            await CreateIndex(db, index);
        }
    }

    private static async Task<IEnumerable<string>> GetIndexes(IDatabase db)
    {
        var result = await db.ExecuteAsync("FT._LIST");
        if (result.IsNull || result.Type != ResultType.MultiBulk)
        {
            throw new PlatformException("Could not list redis indexes");
        }
        var indexArray = (RedisResult[])result!;
        var indexes = new List<string>();
        foreach (var index in indexArray)
        {
            var asString = index.ToString();
            if (asString != null)
                indexes.Add(asString);
        }
        return indexes;
    }

    private static async Task DeleteIndex(IDatabase db, IRedisIndex index)
    {
        var result = await db.ExecuteAsync("FT.DROPINDEX", index.Name);
        if (result.ToString() != "OK")
            throw new PlatformException($"Could not drop index {index.Name}");
    }

    private static async Task CreateIndex(IDatabase db, IRedisIndex index)
    {
        var arguments = new List<string> { index.Name, "ON", "JSON", "PREFIX", "1", index.Prefix, "SCHEMA" };
        index.Schema.Split(' ').ToList().ForEach(x => arguments.Add(x));
        var result = await db.ExecuteAsync("FT.CREATE", arguments.ToArray());
        if (result.ToString() != "OK")
            throw new PlatformException($"Could not create index {index.Name}");

        if (!await db.StringSetAsync($"{index.Name}_VERSION", index.Version))
            throw new PlatformException($"Could not set index {index.Name} version");
    }
}
