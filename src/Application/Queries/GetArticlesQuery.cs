
namespace AJE.Application.Queries;

public record GetArticlesQuery : IRequest<PaginatedList<Article>>
{
    public bool OnlyPublished { get; init; }
    public int Offset { get; init; }
    public int PageSize { get; init; }
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
        var query = request.OnlyPublished ? "@published:{true}" : "*";
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
