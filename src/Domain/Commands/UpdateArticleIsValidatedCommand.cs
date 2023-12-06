namespace AJE.Domain;

public record UpdateArticleIsValidatedCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required bool IsValidated { get; init; }
}

public class UpdateArticleIsValidatedCommandHandler : IRequestHandler<UpdateArticleIsValidatedCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public UpdateArticleIsValidatedCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(UpdateArticleIsValidatedCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateIsValidated(command.Id, command.IsValidated);
        var e = new ArticleUpdatedEvent
        {
            Id = command.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Published = false,
            ContentUpdated = false,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}