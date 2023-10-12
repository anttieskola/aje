namespace AJE.Application.Queries;

public record ArticleExistsQuery : IRequest<bool>
{
    public required string Source { get; init; }
}

public class ArticleExistsQueryHandler : IRequestHandler<ArticleExistsQuery, bool>
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly IConnectionMultiplexer _connection;

    public ArticleExistsQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<bool> Handle(ArticleExistsQuery request, CancellationToken cancellationToken)
    {
        var db = _connection.GetDatabase();
        var arguments = new string[] { _index.Name, $"@source:{{{request.Source.RedisEscape()}}}", "NOCONTENT" };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        var resultItems = (RedisResult[])result!;
        return ((int)resultItems[0]) > 0;
    }
}