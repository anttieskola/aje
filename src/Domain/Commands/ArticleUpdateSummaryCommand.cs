namespace AJE.Domain.Commands;

public record ArticleUpdateSummaryCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int SummaryVersion { get; init; }
    public required string Summary { get; init; }
}

public class ArticleUpdateSummaryCommandHandler : IRequestHandler<ArticleUpdateSummaryCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateSummaryCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateSummaryCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateSummaryAsync(command.Id, command.SummaryVersion, command.Summary);
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