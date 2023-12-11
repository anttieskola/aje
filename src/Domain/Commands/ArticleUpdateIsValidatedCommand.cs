namespace AJE.Domain.Commands;

public record ArticleUpdateIsValidatedCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required bool IsValidated { get; init; }
}

public class ArticleUpdateIsValidatedCommandHandler : IRequestHandler<ArticleUpdateIsValidatedCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateIsValidatedCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateIsValidatedCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateIsValidatedAsync(command.Id, command.IsValidated);
        var e = new ArticleUpdatedEvent
        {
            Id = command.Id,
            Timestamp = DateTimeOffset.UtcNow,
            ContentUpdated = false,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}