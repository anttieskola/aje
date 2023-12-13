namespace AJE.Domain.Commands;

public record ArticleUpdatePolarityCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int PolarityVersion { get; init; }
    public required Polarity Polarity { get; init; }
}

public class ArticleUpdatePolarityCommandHandler : IRequestHandler<ArticleUpdatePolarityCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdatePolarityCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdatePolarityCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdatePolarityAsync(command.Id, command.PolarityVersion, command.Polarity);
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