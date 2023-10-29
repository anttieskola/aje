namespace AJE.Domain.Queries;

public record GetArticlePolarityQuery : IRequest<ArticleClassifiedEvent>
{
    /// <summary>
    /// Current of the polarity model & prompt settings
    /// </summary>
    public const int CURRENT_POLARITY_VERSION = 1;
    public required Article Article { get; init; }
}

public class GetArticlePolarityQueryHandler : IRequestHandler<GetArticlePolarityQuery, ArticleClassifiedEvent>
{

    private readonly IContextCreator<Article> _contextCreator;
    private readonly IPolarity _polarity;
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;

    public GetArticlePolarityQueryHandler(
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

    public async Task<ArticleClassifiedEvent> Handle(GetArticlePolarityQuery command, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(command.Article);
        var prompt = _polarity.Create(context);

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
            var fileNamePrefix = command.Article.Source.Replace("https://yle.fi/a/", string.Empty);
            await _aiLogger.LogAsync(fileNamePrefix, request, response);
        }

        return new ArticleClassifiedEvent
        {
            Timestamp = DateTimeOffset.UtcNow,
            Source = command.Article.Source,
            Polarity = polarity,
            PolarityVersion = GetArticlePolarityQuery.CURRENT_POLARITY_VERSION,
        };
    }
}