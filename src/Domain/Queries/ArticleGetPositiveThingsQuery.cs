namespace AJE.Domain.Queries;

public record ArticleGetPositiveThingsQuery : IRequest<string>
{
    public const int CURRENT_POSITIVE_THINGS_VERSION = 1;
    public required Article Article { get; init; }
}

public class ArticleGetPositiveThingsQueryHandler : IRequestHandler<ArticleGetPositiveThingsQuery, string>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly PositiveThingsChatML _positiveThingsChatML = new();
    private readonly WhatLanguageChatML _whatLanguageChatML = new();
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;
    public ArticleGetPositiveThingsQueryHandler(
        IContextCreator<Article> contextCreator,
        IAiModel aiModel,
        IAiLogger aiLogger)
    {
        _contextCreator = contextCreator;
        _aiModel = aiModel;
        _aiLogger = aiLogger;
    }

    public async Task<string> Handle(ArticleGetPositiveThingsQuery query, CancellationToken cancellationToken)
    {
        // never gonna give you up
        // never gonna let you down
        int tries = 1;
        while (true)
        {
            // create list of positive things
            var context = _contextCreator.Create(query.Article);
            var prompt = _positiveThingsChatML.Context(context);
            var settings = CompletionAdjustor.GetSettings(tries);
            var summaryRequest = new CompletionRequest
            {
                Prompt = prompt,
                Temperature = settings.Temperature,
                TopK = settings.TopK,
                Stop = _positiveThingsChatML.StopWords,
                NumberOfTokensToPredict = 16192,
            };
            var summaryResponse = await _aiModel.CompletionAsync(summaryRequest, cancellationToken);

            // check if summary is in english
            var languageRequest = new CompletionRequest
            {
                Prompt = _whatLanguageChatML.Context(summaryResponse.Content),
                Temperature = 0.1,
                Stop = _whatLanguageChatML.StopWords,
                NumberOfTokensToPredict = 16,
            };
            // need make web server from lingua-rs and just use it
            var languageResponse = await _aiModel.CompletionAsync(languageRequest, cancellationToken);
            if (languageResponse.Content.Contains("english", StringComparison.OrdinalIgnoreCase))
            {
                return summaryResponse.Content;
            }
            _aiLogger.Log($"Positive things list was not in english, retrying... Tries:{tries} Article id:{query.Article.Id} source:{query.Article.Source}");
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            tries++;
        }
    }
}