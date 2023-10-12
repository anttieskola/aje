namespace AJE.Application.Queries;

public record GetArticleHeadersQuery : IRequest<PaginatedList<ArticleHeader>>
{
    public bool OnlyPublished { get; init; }
    public int Offset { get; init; }
    public int PageSize { get; init; }
}

public class GetArticleHeadersQueryHandler : IRequestHandler<GetArticleHeadersQuery, PaginatedList<ArticleHeader>>
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;

    public GetArticleHeadersQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<PaginatedList<ArticleHeader>> Handle(GetArticleHeadersQuery request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        var query = request.OnlyPublished ? "@published:{true}" : "*";
        var arguments = new string[] { _index.Name, query, "SORTBY", "modified", "DESC", "RETURN", "2", "$.id", "$.title", "LIMIT", request.Offset.ToString(), request.PageSize.ToString() };
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

        return new PaginatedList<ArticleHeader>(list, request.Offset, totalCount);
    }
}
