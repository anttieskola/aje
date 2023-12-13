namespace AJE.Domain.Queries;

public record ArticleSearchHeadersQuery : PaginatedQuery, IRequest<PaginatedList<ArticleHeader>>
{
    public required string Keyword { get; init; }
}

public class ArticleSearchHeadersQueryHandler : IRequestHandler<ArticleSearchHeadersQuery, PaginatedList<ArticleHeader>>
{
    private readonly IArticleRepository _repository;

    public ArticleSearchHeadersQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<ArticleHeader>> Handle(ArticleSearchHeadersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHeadersAsync(request);
    }
}