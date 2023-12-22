namespace AJE.Domain.Queries;

public record ArticlesGetSummaryQuery : IRequest<string>
{
    public required IEnumerable<Article> Articles { get; init; }
}

public class ArticlesGetSummaryQueryHandler : IRequestHandler<ArticlesGetSummaryQuery, string>
{
    private readonly SummaryOfSummariesChatML _summaryOfSummariesChatML = new();
    private readonly IAiModel _aiModel;
    public ArticlesGetSummaryQueryHandler(
        IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<string> Handle(ArticlesGetSummaryQuery query, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        foreach (var article in query.Articles)
        {
            if (!string.IsNullOrEmpty(article.Analysis.Summary))
            {
                sb.AppendLine(article.Title);
                sb.Append(article.Analysis.Summary);
                sb.Append('\n');
            }
        }
        var context = sb.ToString();

        var prompt = _summaryOfSummariesChatML.Context(context);
        var summaryRequest = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _summaryOfSummariesChatML.StopWords,
            NumberOfTokensToPredict = 16192,
        };
        var summaryResponse = await _aiModel.CompletionAsync(summaryRequest, cancellationToken);
        return summaryResponse.Content;
    }
}