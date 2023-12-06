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
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _checkArticle.StopWords,
        };

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
                    IsArticle = true,
                };
            }
            throw;
        }

        try
        {
            var isArticle = _checkArticle.Parse(response.Content);
            if (!isArticle)
            {
                return new CheckArticleResult
                {
                    Id = query.Article.Id,
                    IsArticle = false,
                    Reasoning = _checkArticle.ParseReasoning(response.Content),
                };
            }
            return new CheckArticleResult
            {
                Id = query.Article.Id,
                IsArticle = true,
            };
        }
        catch (AiException)
        {
            await _aiLogger.LogAsync($"CheckArticle-{query.Article.Id.ToString()}", request, response);
            throw;
        }
    }

}