
namespace AJE.Application.Queries;

public record GetArticlesQuery : PaginatedQuery, IRequest<PaginatedList<Article>>
{
    public Category? Category { get; init; }
    public bool? Published { get; init; }
    public string? Language { get; init; }
    public Polarity? Polarity { get; init; }
    public int? MaxPolarityVersion { get; init; }
}

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, PaginatedList<Article>>
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;

    public GetArticlesQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<PaginatedList<Article>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();

        var builder = new QueryBuilder();
        if (request.Category != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@category:[{(int)request.Category} {(int)request.Category}]" });
        if (request.Published != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@published:{{{request.Published}}}" });
        if (request.Language != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@language:{{{request.Language}}}" });
        if (request.Polarity != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarity:{{{request.Polarity}}}" });
        if (request.MaxPolarityVersion != null)
            builder.Conditions.Add(new QueryCondition { Expression = $"@polarityVersion:[-inf {request.MaxPolarityVersion}]" });
        var query = builder.Build();

        var arguments = new string[] { _index.Name, query, "SORTBY", "modified", "DESC", "LIMIT", request.Offset.ToString(), request.PageSize.ToString() };
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
        return new PaginatedList<Article>(list, request.Offset, totalCount);
    }
}
