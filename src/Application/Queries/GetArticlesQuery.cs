namespace AJE.Application.Queries;

public record GetArticlesQuery : IRequest<Article>
{
    public bool OnlyPublished { get; init; }
    public int Offset { get; init; }
    public int PageSize { get; init; }
}

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, Article>
{
    private IConnectionMultiplexer _connection;

    public GetArticlesQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<Article> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var ft = _connection.GetDatabase().FT();

        var query = new Query(request.OnlyPublished ? "@published:{true}" : "*")
            .Limit(request.Offset, request.PageSize)
            .ReturnFields(new FieldName("id"), new FieldName("title"))
            .Dialect(3);

        var result = await ft.SearchAsync(ArticleConstants.INDEX_NAME, query);
        var headers = from doc in result.Documents
                      select Parse(doc);

        return new Article();
    }

    private static ArticleHeader Parse(Document document)
    {
        var id = document["id"].ToString();
        var title = document["title"].ToString();
        var IdArray = JsonSerializer.Deserialize<Guid[]>(id) ?? throw new Exception();
        var titleArray = JsonSerializer.Deserialize<string[]>(title) ?? throw new Exception();
        if (IdArray.Length != 1 || titleArray.Length != 1)
        {
            throw new Exception();
        }
        return new ArticleHeader { Id = IdArray[0], Title = titleArray[0] };
    }
}
