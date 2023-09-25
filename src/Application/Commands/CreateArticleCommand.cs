namespace AJE.Application.Commands;

public record CreateArticleCommand : IRequest<ArticleCreatedEvent>
{
}

public class CreateArticle : IRequestHandler<CreateArticleCommand, ArticleCreatedEvent>
{
    private IConnectionMultiplexer _connection;

    public CreateArticle(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public Task<ArticleCreatedEvent> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
