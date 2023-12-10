namespace AJE.Domain.Commands;

public record ArticleUpdateTokenCountCommand : IRequest<ArticleUpdatedEvent>
{
    public required Guid Id { get; init; }
    public required int TokenCount { get; init; }
}

public class ArticleUpdateTokenCountCommandHandler : IRequestHandler<ArticleUpdateTokenCountCommand, ArticleUpdatedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleUpdateTokenCountCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleUpdatedEvent> Handle(ArticleUpdateTokenCountCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.UpdateTokenCountAsync(command.Id, command.TokenCount);
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