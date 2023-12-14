namespace AJE.Domain.Commands;

public record ArticleUpdatePositiveThingsCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int PositiveThingsVersion { get; init; }
    public required string PositiveThings { get; init; }
}

public class ArticleUpdatePositiveThingsCommandHandler : IRequestHandler<ArticleUpdatePositiveThingsCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdatePositiveThingsCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdatePositiveThingsCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdatePositiveThingsAsync(command.Id, command.PositiveThingsVersion, command.PositiveThings);
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