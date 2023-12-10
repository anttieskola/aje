namespace AJE.Infra.Redis.Data;

public class ArticleRepository : IArticleRepository
{
    private readonly ArticleIndex _index = new();
    private readonly ILogger<ArticleRepository> _logger;
    private readonly IConnectionMultiplexer _connection;

    public ArticleRepository(
        ILogger<ArticleRepository> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    #region modifications

    public async Task AddAsync(Article article)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(article.Id.ToString());
        if (await db.KeyExistsAsync(redisId))
            throw new KeyExistsException(redisId);

        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(article));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set value {redisId}");

        _logger.LogTrace("Added article {Id}", article.Id);
    }

    public async Task UpdateAsync(Article article)
    {
        var current = await GetAsync(article.Id);
        if (current == article)
            throw new ArgumentException("article is not changed", nameof(article));

        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(article.Id.ToString());

        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$", JsonSerializer.Serialize(article));
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set value {redisId}");

        _logger.LogTrace("Updated article {Id}", article.Id);
    }

    public async Task UpdateIsValidatedAsync(Guid id, bool isValidated)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());
        var isValidatedJson = JsonSerializer.Serialize(isValidated);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.isValidated", isValidatedJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set IsValidated on article {redisId}");
    }

    public async Task UpdateTokenCountAsync(Guid id, int tokenCount)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());
        var tokenCountJson = JsonSerializer.Serialize(tokenCount);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.tokenCount", tokenCountJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set IsValidated on article {redisId}");
    }

    #endregion modifications

    #region queries
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

    public async Task<PaginatedList<Article>> GetAsync(ArticleGetManyQuery query)
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
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:[{(int)query.Polarity} {(int)query.Polarity}]" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        if (query.IsValidated != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@isValidated:{{{query.IsValidated}}}" });
        if (query.MaxTokenCount != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@tokenCount:[-inf {query.MaxTokenCount}]" });
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

    public async Task<Article> GetBySourceAsync(string source)
    {
        var db = _connection.GetDatabase();
        var arguments = new string[] { _index.Name, $"@source:{{{source.RedisEscape()}}}", "LIMIT", "0", "1" };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        var resultItems = (RedisResult[])result!;
        if (resultItems.Length == 3)
        {
            var data = resultItems[2] ?? throw new DataException($"invalid value in key {resultItems[1]}");
            var json = ((RedisResult[])data!)[1].ToString() ?? throw new DataException($"invalid value in key {resultItems[1]}");
            var article = JsonSerializer.Deserialize<Article>(json);
            return article ?? throw new DataException($"invalid value in key {resultItems[1]}");
        }
        throw new KeyNotFoundException($"Article with source {source} not found");
    }

    public async Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleGetHeadersQuery query)
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
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:[{(int)query.Polarity} {(int)query.Polarity}]" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        var queryString = builder.Build();

        var arguments = new string[] { _index.Name, queryString, "SORTBY", "modified", "DESC", "RETURN", "3", "$.id", "$.title", "$.polarity", "LIMIT", query.Offset.ToString(), query.PageSize.ToString() };
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
            if (data.Length == 4)
            {
                var id = (string)data[1]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var title = (string)data[3]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var polarity = Polarity.Unknown;
                list.Add(new ArticleHeader { Id = Guid.Parse(id), Title = title, Polarity = polarity });
            }
            else if (data.Length == 6)
            {
                var id = (string)data[1]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var title = (string)data[3]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                var polarity = (int?)data[5]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                list.Add(new ArticleHeader { Id = Guid.Parse(id), Title = title, Polarity = (Polarity)polarity });
            }
            else
            {
                _logger.LogError("invalid data value in key:{}", rows[i]);
                _logger.LogError("invalid data Length:{}", data.Length);
                for (var e = 0; e < data.Length; e++)
                    _logger.LogError("invalid data Index:{} Item:{}", e, data[e]);

                throw new DataException($"invalid data value in key {rows[i]}"); // here it crashes
            }
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

    #endregion queries
}
