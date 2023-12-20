namespace AJE.Domain.Queries;

public record ArticleGetPositiveThingsQuery : IRequest<string>
{
    public const int CURRENT_POSITIVE_THINGS_VERSION = 2;
    public required Article Article { get; init; }
}

public class ArticleGetPositiveThingsQueryHandler : IRequestHandler<ArticleGetPositiveThingsQuery, string>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly PositiveThingsChatML _positiveThingsChatML = new();
    private readonly IAiModel _aiModel;
    public ArticleGetPositiveThingsQueryHandler(
        IContextCreator<Article> contextCreator,
        IAiModel aiModel)
    {
        _contextCreator = contextCreator;
        _aiModel = aiModel;
    }

    public async Task<string> Handle(ArticleGetPositiveThingsQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var prompt = _positiveThingsChatML.Context(context);
        var positiveThingsRequest = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _positiveThingsChatML.StopWords,
            NumberOfTokensToPredict = 16192,
        };
        var summaryResponse = await _aiModel.CompletionAsync(positiveThingsRequest, cancellationToken);
        return summaryResponse.Content;
    }
}