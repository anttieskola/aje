namespace AJE.Domain;

public record CheckArticleQuery : IRequest<CheckArticleResult>
{
    public required Article Article { get; init; }
}

public class CheckArticleQueryHandler : IRequestHandler<CheckArticleQuery, CheckArticleResult>
{
    private readonly IContextCreator<Article> _contextCreator;
    private readonly ICheckArticle _checkArticle;
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;

    public CheckArticleQueryHandler(
        IContextCreator<Article> contextCreator,
        ICheckArticle checkArticle,
        IAiModel aiModel,
        IAiLogger aiLogger)
    {
        _contextCreator = contextCreator;
        _checkArticle = checkArticle;
        _aiModel = aiModel;
        _aiLogger = aiLogger;
    }

    public async Task<CheckArticleResult> Handle(CheckArticleQuery query, CancellationToken cancellationToken)
    {
        var context = _contextCreator.Create(query.Article);
        var prompt = _checkArticle.Context(context);

        // could start with temperature 0.1 and increase it if no response
        double temperature = 0.0;
        while (temperature < 0.8)
        {
            temperature += 0.1;
            var request = new CompletionRequest
            {
                Prompt = prompt,
                NumberOfTokensToKeep = 0,
                NumberOfTokensToPredict = 4096,
                Temperature = temperature,
                Stop = _checkArticle.StopWords,
            };

            // Completion with AI
            CompletionResponse response;
            try
            {
                response = await _aiModel.CompletionAsync(request, cancellationToken);
            }
            catch (AiException aex)
            {
                // too large context must be valid article
                if (aex.Message.Contains("Content is too large"))
                {
                    return new CheckArticleResult
                    {
                        Id = query.Article.Id,
                        IsValid = true,
                    };
                }
                continue;
            }

            // Try parse response
            try
            {
                var result = _checkArticle.Parse(response.Content);
                return new CheckArticleResult
                {
                    Id = query.Article.Id,
                    IsValid = result.IsValid,
                    Reasoning = result.Reasoning,
                };
            }
            catch (AiException)
            {
                await _aiLogger.LogAsync($"CheckArticle-{query.Article.Id}", request, response);
            }
        }
        throw new AiException("Failed to check article");
    }

}