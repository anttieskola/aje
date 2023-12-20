namespace AJE.Domain.Queries;

public record ArticleGetSummaryQuery : IRequest<string>
{
    public const int CURRENT_SUMMARY_VERSION = 3;
    public required Article Article { get; init; }
}

public class ArticleGetSummaryQueryHandler : IRequestHandler<ArticleGetSummaryQuery, string>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly SummaryChatML _summaryChatML = new();
    private readonly IAiModel _aiModel;
    public ArticleGetSummaryQueryHandler(
        IContextCreator<Article> contextCreator,
        IAiModel aiModel)
    {
        _contextCreator = contextCreator;
        _aiModel = aiModel;
    }

    public async Task<string> Handle(ArticleGetSummaryQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var prompt = _summaryChatML.Context(context);
        var summaryRequest = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _summaryChatML.StopWords,
            NumberOfTokensToPredict = 16192,
        };
        var summaryResponse = await _aiModel.CompletionAsync(summaryRequest, cancellationToken);
        return summaryResponse.Content;
    }
}