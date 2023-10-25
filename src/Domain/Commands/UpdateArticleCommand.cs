namespace AJE.Domain;

public class UpdateArticleCommand : IRequest<ArticleEvent>
{
    public required Article Article { get; init; }
}

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, ArticleEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public UpdateArticleCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleEvent> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateAsync(command.Article);
        var e = new ArticleEvent
        {
            Id = command.Article.Id,
            Type = ArticleEventType.Updated,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}