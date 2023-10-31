namespace AJE.Domain.Commands;

public class UpdateArticleCommand : IRequest<ArticleUpdatedEvent>
{
    public required Article Article { get; init; }
}

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, ArticleUpdatedEvent>
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

    public async Task<ArticleUpdatedEvent> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateAsync(command.Article);
        var e = new ArticleUpdatedEvent
        {
            Id = command.Article.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Published = command.Article.Published,
            ContentUpdated = true,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}