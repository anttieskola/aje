namespace AJE.Domain.Queries;

public record ArticleGetSentimentPolarityQuery : IRequest<ArticleSentimentPolarity>
{
    public const int CURRENT_POLARITY_VERSION = 2;
    public required Article Article { get; init; }
}

public class ArticleGetSentimentPolarityQueryHandler : IRequestHandler<ArticleGetSentimentPolarityQuery, ArticleSentimentPolarity>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly IPolarity _polarity;
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;

    public ArticleGetSentimentPolarityQueryHandler(
        IContextCreator<Article> contextCreator,
        IPolarity polarity,
        IAiModel aiModel,
        IAiLogger aiLogger)
    {
        _contextCreator = contextCreator;
        _polarity = polarity;
        _aiModel = aiModel;
        _aiLogger = aiLogger;
    }

    public async Task<ArticleSentimentPolarity> Handle(ArticleGetSentimentPolarityQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var prompt = _polarity.Context(context);
        // update version if prompt changes
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _polarity.StopWords,
            NumberOfTokensToPredict = 256,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        var polarity = _polarity.Parse(response.Content);

        // detailed logging for unknown polarity
        if (polarity == Polarity.Unknown)
        {
            // TODO: Only works for YLE's articles
            var fileNamePrefix = query.Article.Source.Replace("https://yle.fi/a/", string.Empty);
            await _aiLogger.LogAsync(fileNamePrefix, request, response);
        }

        return new ArticleSentimentPolarity
        {
            Id = query.Article.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Source = query.Article.Source,
            Polarity = polarity,
            PolarityVersion = ArticleGetSentimentPolarityQuery.CURRENT_POLARITY_VERSION,
        };
    }
}
