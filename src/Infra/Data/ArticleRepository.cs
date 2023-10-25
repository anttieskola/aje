namespace AJE.Infra.Data;

public class ArticleRepository : IArticleRepository
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly ILogger<ArticleRepository> _logger;
    private readonly IConnectionMultiplexer _connection;

    public ArticleRepository(
        ILogger<ArticleRepository> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task AddAsync(Article article)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(article.Id.ToString());
        if (await db.KeyExistsAsync(redisId))
        {
            throw new KeyExistsException(redisId);
        }
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(article));
        if (setResult.ToString() != "OK")
        {
            throw new DataException($"failed to set value {redisId}");
        }
        _logger.LogTrace("Added article {Id}", article.Id);
    }

    public async Task<Article> GetAsync(Guid id)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());
        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"Article with id {id} not found");
        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var article = JsonSerializer.Deserialize<Article>(json);
        return article ?? throw new DataException($"invalid value in key {redisId}");
    }

    public async Task<PaginatedList<Article>> GetAsync(GetArticlesQuery query)
    {
        var db = _connection.GetDatabase();

        var builder = new QueryBuilder();
        if (query.Category != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@category:[{(int)query.Category} {(int)query.Category}]" });
        if (query.Published != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@published:{{{query.Published}}}" });
        if (query.Language != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{query.Language}}}" });
        if (query.Polarity != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:{{{query.Polarity}}}" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        var queryString = builder.Build();

        var arguments = new string[] { _index.Name, queryString, "SORTBY", "modified", "DESC", "LIMIT", query.Offset.ToString(), query.PageSize.ToString() };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        // first item is total count (integer)
        // then pairs of key (bulk string) and value (multibulk)
        // inside value we have: modified, modified-value, json path, json-value
        var rows = (RedisResult[])result!;
        var totalCount = (long)rows[0];
        var list = new List<Article>();
        for (long i = 1; i < rows.LongLength; i += 2)
        {
            var data = (RedisResult[])rows[i + 1]!;
            var article = JsonSerializer.Deserialize<Article>((string)data[3]!)
                ?? throw new DataException($"invalid json value in key {rows[i]}");
            list.Add(article);
        }
        return new PaginatedList<Article>(list, query.Offset, totalCount);
    }

    public async Task<PaginatedList<ArticleHeader>> GetHeadersAsync(GetArticleHeadersQuery query)
    {
        var db = _connection.GetDatabase();

        var builder = new QueryBuilder();
        if (query.Category != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@category:[{(int)query.Category} {(int)query.Category}]" });
        if (query.Published != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@published:{{{query.Published}}}" });
        if (query.Language != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{query.Language}}}" });
        if (query.Polarity != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:{{{query.Polarity}}}" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        var queryString = builder.Build();

        var arguments = new string[] { _index.Name, queryString, "SORTBY", "modified", "DESC", "RETURN", "2", "$.id", "$.title", "LIMIT", query.Offset.ToString(), query.PageSize.ToString() };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        // first item is total count (integer)
        var rows = (RedisResult[])result!;
        var totalCount = (long)rows[0];
        var list = new List<ArticleHeader>();
        // then pairs of key (bulk string) and value (multibulk)
        for (long i = 1; i < rows.LongLength; i += 2)
        {
            // value is in this case defined in return statement (with labels)
            // $.id, id-value, $.title, title-value
            var data = (RedisResult[])rows[i + 1]!;
            var id = (string)data[1]! ?? throw new DataException($"invalid data value in key {rows[i]}");
            var title = (string)data[3]! ?? throw new DataException($"invalid data value in key {rows[i]}");
            list.Add(new ArticleHeader { Id = Guid.Parse(id), Title = title });
        }

        return new PaginatedList<ArticleHeader>(list, query.Offset, totalCount);
    }

    public async Task<bool> ExistsAsync(string source)
    {
        var db = _connection.GetDatabase();
        var arguments = new string[] { _index.Name, $"@source:{{{source.RedisEscape()}}}", "NOCONTENT" };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        var resultItems = (RedisResult[])result!;
        return ((int)resultItems[0]) > 0;
    }
}
