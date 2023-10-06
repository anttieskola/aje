namespace AJE.Application.Queries;

public record GetLatestArticlesQuery : IRequest<IEnumerable<Guid>>
{
    public required int Count { get; init; }
}

public class GetLatestArticlesQueryHandler : IRequestHandler<GetLatestArticlesQuery, IEnumerable<Guid>>
{
    private readonly IConnectionMultiplexer _connection;

    public GetLatestArticlesQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Guid>> Handle(GetLatestArticlesQuery request, CancellationToken cancellationToken)
    {
        var ft = _connection.GetDatabase().FT();

        var query = new Query("*")
            .Limit(0, request.Count)
            .ReturnFields(new FieldName("id"))
            .SetSortBy("modified")
            .Dialect(3);

        var result = await ft.SearchAsync(ArticleConstants.INDEX_NAME, query);
        var ids = from doc in result.Documents
                  select ParseId(doc);

        return ids.ToArray();
    }

    private static Guid ParseId(Document document)
    {
        var id = document["id"].ToString();
        var IdArray = JsonSerializer.Deserialize<Guid[]>(id) ?? throw new ParsingException("could not parse id array");
        if (IdArray.Length != 1)
        {
            throw new ParsingException("id array length is not 1");
        }
        return IdArray[0];
    }
}
