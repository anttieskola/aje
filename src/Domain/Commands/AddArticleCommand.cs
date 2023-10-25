namespace AJE.Domain.Commands;

public record AddArticleCommand : IRequest<ArticleEvent>
{
    public required Article Article { get; init; }
}

public class AddArticleCommandHandler : IRequestHandler<AddArticleCommand, ArticleEvent>
{
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleEventHandler _articleEventHandler;

    public AddArticleCommandHandler(
        IArticleRepository articleRepository,
        IArticleEventHandler articleEventHandler)
    {
        _articleRepository = articleRepository;
        _articleEventHandler = articleEventHandler;
    }

    public async Task<ArticleEvent> Handle(AddArticleCommand command, CancellationToken cancellationToken)
    {
        await _articleRepository.AddAsync(command.Article);
        var e = new ArticleEvent
        {
            Id = command.Article.Id,
            Type = ArticleEventType.Added,
        };
        await _articleEventHandler.SendAsync(e);
        return e;
    }
}