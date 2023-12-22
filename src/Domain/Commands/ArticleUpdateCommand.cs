namespace AJE.Domain.Commands;

public class ArticleUpdateCommand : IRequest<ArticleUpdatedEvent>
{
    public required Article Article { get; init; }
    public required bool ContentUpdated { get; init; }
}

public class ArticleUpdateCommandHandler : IRequestHandler<ArticleUpdateCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateAsync(command.Article);
        var e = new ArticleUpdatedEvent
        {
            Id = command.Article.Id,
            Timestamp = DateTimeOffset.UtcNow,
            ContentUpdated = command.ContentUpdated,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}