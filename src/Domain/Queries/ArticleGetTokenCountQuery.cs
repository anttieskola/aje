namespace AJE.Domain.Queries;

public record ArticleGetTokenCountQuery : IRequest<int>
{
    public required Article Article { get; init; }
}

public class ArticleGetTokenCountHandler : IRequestHandler<ArticleGetTokenCountQuery, int>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly IAiModel _aiModel;

    public ArticleGetTokenCountHandler(
        IContextCreator<Article> contextCreator,
        IAiModel aiModel)
    {
        _contextCreator = contextCreator;
        _aiModel = aiModel;
    }

    public async Task<int> Handle(ArticleGetTokenCountQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var t = await _aiModel.TokenizeAsync(new TokenizeRequest
        {
            Content = context
        }, cancellationToken);
        return t.Tokens.Length;
    }
}