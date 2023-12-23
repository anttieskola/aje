namespace AJE.Domain.Commands;

public record ArticleUpdateCorporationsCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int CorporationsVersion { get; init; }
    public required EquatableList<Corporation> Corporations { get; init; }
}

public class ArticleUpdateCorporationsCommandHandler : IRequestHandler<ArticleUpdateCorporationsCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateCorporationsCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateCorporationsCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateCorporationsAsync(command.Id, command.CorporationsVersion, command.Corporations);
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
