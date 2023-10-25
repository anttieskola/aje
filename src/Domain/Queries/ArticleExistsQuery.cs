namespace AJE.Domain.Queries;

public record ArticleExistsQuery : IRequest<bool>
{
    public required string Source { get; init; }
}

public class ArticleExistsQueryHandler : IRequestHandler<ArticleExistsQuery, bool>
{
    private readonly IArticleRepository _repository;

    public ArticleExistsQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ArticleExistsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(request.Source);
    }
}