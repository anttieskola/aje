namespace AJE.Domain.Commands;

public record ArticleUpdateKeyPeopleCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int KeyPeopleVersion { get; init; }
    public required EquatableList<KeyPerson> KeyPeople { get; init; }
}

public class ArticleUpdateKeyPeopleCommandHandler : IRequestHandler<ArticleUpdateKeyPeopleCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateKeyPeopleCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateKeyPeopleCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateKeyPeopleAsync(command.Id, command.KeyPeopleVersion, command.KeyPeople);
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