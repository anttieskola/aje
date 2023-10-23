namespace AJE.Domain.Commands;

public record PublishArticleCommand : IRequest<ArticlePublishedEvent>
{
    public required Article Article { get; init; }
}
