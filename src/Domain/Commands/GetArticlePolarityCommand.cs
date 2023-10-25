﻿namespace AJE.Domain.Commands;

public record GetArticlePolarityCommand : IRequest<ArticleClassifiedEvent>
{
    /// <summary>
    /// Current of the polarity model & prompt settings
    /// </summary>
    public const int CURRENT_POLARITY_VERSION = 1;
    public required Article Article { get; init; }
}

public class GetArticlePolarityCommandHandler : IRequestHandler<GetArticlePolarityCommand, ArticleClassifiedEvent>
{

    private readonly IContextCreator<Article> _contextCreator;
    private readonly IPolarity _polarity;
    private readonly IAiModel _aiModel;

    public GetArticlePolarityCommandHandler(
        IContextCreator<Article> contextCreator,
        IPolarity polarity,
        IAiModel aiModel)
    {
        _contextCreator = contextCreator;
        _polarity = polarity;
        _aiModel = aiModel;
    }

    public async Task<ArticleClassifiedEvent> Handle(GetArticlePolarityCommand command, CancellationToken cancellationToken)
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
        return new ArticleClassifiedEvent
        {
            Id = command.Article.Id,
            Polarity = polarity,
            PolarityVersion = GetArticlePolarityCommand.CURRENT_POLARITY_VERSION,
        };
    }
}