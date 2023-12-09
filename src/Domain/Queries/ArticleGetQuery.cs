
namespace AJE.Domain.Queries;

#region ById

public record ArticleGetByIdQuery : IRequest<Article>
{
    public required Guid Id { get; init; }
}

public class ArticleGetByIdQueryHandler : IRequestHandler<ArticleGetByIdQuery, Article>
{
    private readonly IArticleRepository _repository;

    public ArticleGetByIdQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> Handle(ArticleGetByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.Id);
    }
}

#endregion ById

#region BySource

public record ArticleGetBySourceQuery : IRequest<Article>
{
    public required string Source { get; init; }
}

public class ArticleGetBySourceQueryHandler : IRequestHandler<ArticleGetBySourceQuery, Article>
{
    private readonly IArticleRepository _repository;

    public ArticleGetBySourceQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> Handle(ArticleGetBySourceQuery request, CancellationToken cancellationToken)
    {

        return await _repository.GetBySourceAsync(request.Source);
    }
}

#endregion BySource