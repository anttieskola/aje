namespace AJE.Application.Queries;

public record GetArticleQuery : IRequest<Article>
{
    public required Guid Id { get; init; }
}

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, Article>
{
    private readonly IConnectionMultiplexer _connection;

    public GetArticleQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var json = _connection.GetDatabase().JSON();

        var redisId = $"{ArticleConstants.INDEX_PREFIX}{request.Id}";
        var result = (string?)await json.GetAsync(redisId);
        if (result != null)
        {
            var article = JsonSerializer.Deserialize<Article>(result);
            if (article != null)
                return article;
        }
        throw new Exception("todo");
    }
}