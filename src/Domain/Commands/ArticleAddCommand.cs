namespace AJE.Domain.Commands;

public record ArticleAddCommand : IRequest<ArticleAddedEvent>
{
    public required Article Article { get; init; }
}

public class ArticleAddCommandHandler : IRequestHandler<ArticleAddCommand, ArticleAddedEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public ArticleAddCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleAddedEvent> Handle(ArticleAddCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.AddAsync(command.Article);
        var e = new ArticleAddedEvent
        {
            Id = command.Article.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Published = command.Article.Published,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}