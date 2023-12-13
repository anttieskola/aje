namespace AJE.Domain.Queries;

public record ArticleGetHeadersQuery : PaginatedQuery, IRequest<PaginatedList<ArticleHeader>>
{
    public ArticleCategory? Category { get; init; }
    public string? Language { get; init; }
    public Polarity? Polarity { get; init; }
    public int? MaxPolarityVersion { get; init; }
    public string? Keyword { get; init; }
}

public class ArticleGetHeadersQueryHandler : IRequestHandler<ArticleGetHeadersQuery, PaginatedList<ArticleHeader>>
{
    private readonly IArticleRepository _repository;

    public ArticleGetHeadersQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<ArticleHeader>> Handle(ArticleGetHeadersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHeadersAsync(request);
    }
}