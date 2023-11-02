namespace AJE.Domain.Queries;

public record GetArticleSentimentPolarityQuery : IRequest<ArticleSentimentPolarity>
{
    /// <summary>
    /// Current of the polarity model & prompt settings
    /// </summary>
    public const int CURRENT_POLARITY_VERSION = 1;
    public required Article Article { get; init; }
}

public class GetArticleSentimentPolarityQueryHandler : IRequestHandler<GetArticleSentimentPolarityQuery, ArticleSentimentPolarity>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly IPolarity _polarity;
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;

    public GetArticleSentimentPolarityQueryHandler(
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

    public async Task<ArticleSentimentPolarity> Handle(GetArticleSentimentPolarityQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var prompt = _polarity.Context(context);

        // update version if prompt changes
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
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
            PolarityVersion = GetArticleSentimentPolarityQuery.CURRENT_POLARITY_VERSION,
        };
    }
}
