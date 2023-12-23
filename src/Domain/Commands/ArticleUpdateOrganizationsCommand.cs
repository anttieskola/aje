namespace AJE.Domain.Commands;

public record ArticleUpdateOrganizationsCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int OrganizationsVersion { get; init; }
    public required EquatableList<Organization> Organizations { get; init; }
}

public class ArticleUpdateOrganizationsCommandHandler : IRequestHandler<ArticleUpdateOrganizationsCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateOrganizationsCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateOrganizationsCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateOrganizationsAsync(command.Id, command.OrganizationsVersion, command.Organizations);
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