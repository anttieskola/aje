namespace AJE.Application.Queries;

public record GetArticleQuery : IRequest<Article>
{
    public required Guid Id { get; init; }
}

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, Article>
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;

    public GetArticleQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        var redisId = _index.RedisId(request.Id.ToString());
        var result = await db.ExecuteAsync("JSON.GET", redisId);
        if (result.IsNull)
            throw new KeyNotFoundException($"Article with id {request.Id} not found");
        var json = result.ToString() ?? throw new DataException($"invalid value in key {redisId}");
        var article = JsonSerializer.Deserialize<Article>(json);
        return article ?? throw new DataException($"invalid value in key {redisId}");
    }
}