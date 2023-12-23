namespace AJE.Domain.Commands;

public record ArticleUpdateLocationsCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int LocationsVersion { get; init; }
    public required EquatableList<Location> Locations { get; init; }
}

public class ArticleUpdateLocationsCommandHandler : IRequestHandler<ArticleUpdateLocationsCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateLocationsCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateLocationsCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateLocationsAsync(command.Id, command.LocationsVersion, command.Locations);
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