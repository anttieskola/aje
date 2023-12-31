﻿namespace AJE.Infra.Redis.Data;

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

    public async Task UpdatePolarityAsync(Guid id, int polarityVersion, Polarity polarity)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        // polarityVersion
        var polarityVersionJson = JsonSerializer.Serialize(polarityVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.polarityVersion", polarityVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.polarityVersion on article {redisId}");

        // polarity
        var polarityJson = JsonSerializer.Serialize(polarity);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.polarity", polarityJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.polarity on article {redisId}");
    }

    public async Task UpdateSummaryAsync(Guid id, int summaryVersion, string summary)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var summaryVersionJson = JsonSerializer.Serialize(summaryVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.summaryVersion", summaryVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.summaryVersion on article {redisId}");

        var summaryJson = JsonSerializer.Serialize(summary);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.summary", summaryJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.summary on article {redisId}");
    }

    public async Task UpdatePositiveThingsAsync(Guid id, int positiveThingsVersion, EquatableList<PositiveThing> positiveThings)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var positiveThingsVersionJson = JsonSerializer.Serialize(positiveThingsVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.positiveThingsVersion", positiveThingsVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.positiveThingsVersion on article {redisId}");

        var positiveThingsJson = JsonSerializer.Serialize(positiveThings);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.positiveThings", positiveThingsJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.positiveThings on article {redisId}");
    }

    public async Task UpdateLocationsAsync(Guid id, int locationsVersion, EquatableList<Location> locations)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var locationsVersionJson = JsonSerializer.Serialize(locationsVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.locationsVersion", locationsVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.locationsVersion on article {redisId}");

        var locationsJson = JsonSerializer.Serialize(locations);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.locations", locationsJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.locations on article {redisId}");
    }


    public async Task UpdateCorporationsAsync(Guid id, int corporationsVersion, EquatableList<Corporation> corporations)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var corporationsVersionJson = JsonSerializer.Serialize(corporationsVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.corporationsVersion", corporationsVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.corporationsVersion on article {redisId}");

        var corporationsJson = JsonSerializer.Serialize(corporations);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.corporations", corporationsJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.corporations on article {redisId}");
    }

    public async Task UpdateOrganizationsAsync(Guid id, int organizationsVersion, EquatableList<Organization> organizations)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var organizationsVersionJson = JsonSerializer.Serialize(organizationsVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.organizationsVersion", organizationsVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.organizationsVersion on article {redisId}");

        var organizationsJson = JsonSerializer.Serialize(organizations);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.organizations", organizationsJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.organizations on article {redisId}");
    }

    public async Task UpdateKeyPeopleAsync(Guid id, int keyPeopleVersion, EquatableList<KeyPerson> KeyPeople)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(id.ToString());

        var keyPeopleVersionJson = JsonSerializer.Serialize(keyPeopleVersion);
        var setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.keyPeopleVersion", keyPeopleVersionJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.keyPeopleVersion on article {redisId}");

        var keyPeopleJson = JsonSerializer.Serialize(KeyPeople);
        setResult = await db.ExecuteAsync("JSON.SET", redisId, "$.analysis.keyPeople", keyPeopleJson);
        if (setResult.ToString() != "OK")
            throw new DataException($"failed to set $.analysis.keyPeople on article {redisId}");
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
        if (query.Language != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{query.Language}}}" });
        if (query.Languages != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{string.Join("|", query.Languages)}}}" });
        if (query.Polarity != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:[{(int)query.Polarity} {(int)query.Polarity}]" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        if (query.IsValidForAnalysis != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@isValidForAnalysis:{{{query.IsValidForAnalysis}}}" });
        if (query.IsLiveNews != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@isLiveNews:{{{query.IsLiveNews}}}" });
        if (query.MaxSummaryVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@summaryVersion:[-inf {query.MaxSummaryVersion}]" });
        if (query.MaxPositiveThingsVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@positiveThingsVersion:[-inf {query.MaxPositiveThingsVersion}]" });
        if (query.MaxLocationsVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@locationsVersion:[-inf {query.MaxLocationsVersion}]" });
        if (query.MaxOrganizationsVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@organizationsVersion:[-inf {query.MaxOrganizationsVersion}]" });
        if (query.MaxCorporationsVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@corporationsVersion:[-inf {query.MaxCorporationsVersion}]" });
        if (query.MaxKeyPeopleVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@keyPeopleVersion:[-inf {query.MaxKeyPeopleVersion}]" });
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
        var builder = new QueryBuilder();
        if (query.Category != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@category:[{(int)query.Category} {(int)query.Category}]" });
        if (query.Language != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{query.Language}}}" });
        if (query.Polarity != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:[{(int)query.Polarity} {(int)query.Polarity}]" });
        if (query.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {query.MaxPolarityVersion}]" });
        var queryString = builder.Build();

        return await GetHeadersAsync(queryString, query.Offset, query.PageSize);
    }

    public async Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleSearchHeadersQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            _logger.LogInformation("GetHeadersAsync: {}", query.Keyword);
            return await GetHeadersAsync(query.Keyword, query.Offset, query.PageSize);
        }
        catch (RedisException re)
        {
            _logger.LogError(re, "RedisException in GetHeadersAsync");
            return new PaginatedList<ArticleHeader>(new List<ArticleHeader>(), 0, 0);
        }
    }

    private async Task<PaginatedList<ArticleHeader>> GetHeadersAsync(string queryString, int offset, int pageSize)
    {
        var db = _connection.GetDatabase();
        var arguments = new string[] { _index.Name, queryString, "SORTBY", "modified", "DESC", "RETURN", "3", "$.id", "$.title", "$.polarity", "LIMIT", offset.ToString(), pageSize.ToString() };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        // first item is total count (integer)
        var rows = (RedisResult[])result!;
        var totalCount = (long)rows[0];
        var list = new List<ArticleHeader>();
        // then pairs of key (bulk string) and value (multibulk)
        for (long i = 1; i < rows.LongLength; i += 2)
        {
            // value is in this case defined in return statement (with labels)
            // $.id, id-value, $.title, title-value, $.polarity, polarity-value
            var data = (RedisResult[])rows[i + 1]!;
            if (data.Length == 6)
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

        return new PaginatedList<ArticleHeader>(list, offset, totalCount);
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
