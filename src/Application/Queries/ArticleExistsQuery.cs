namespace AJE.Application.Queries;

public record ArticleExistsQuery : IRequest<bool>
{
    public required string Source { get; init; }
}

public class ArticleExistsQueryHandler : IRequestHandler<ArticleExistsQuery, bool>
{
    private readonly IConnectionMultiplexer _connection;

    public ArticleExistsQueryHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<bool> Handle(ArticleExistsQuery request, CancellationToken cancellationToken)
    {   
        var ft = _connection.GetDatabase().FT();

        var sourceKeyword = request.Source.RedisEscape();
        var query = new Query($"@source:{{{sourceKeyword}}}")
            .Limit(0, 1)
            .ReturnFields(new FieldName("id"))
            .Dialect(3);

        var results = await ft.SearchAsync(ArticleConstants.INDEX_NAME, query);
        return results.TotalResults > 0;
    }    
}