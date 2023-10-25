namespace AJE.Domain.Queries;

public record GetArticleHeadersQuery : PaginatedQuery, IRequest<PaginatedList<ArticleHeader>>
{
    public Category? Category { get; init; }
    public bool? Published { get; init; }
    public string? Language { get; init; }
    public Polarity? Polarity { get; init; }
    public int? MaxPolarityVersion { get; init; }
}

public class GetArticleHeadersQueryHandler : IRequestHandler<GetArticleHeadersQuery, PaginatedList<ArticleHeader>>
{
    private readonly IArticleRepository _repository;

    public GetArticleHeadersQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<ArticleHeader>> Handle(GetArticleHeadersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetHeadersAsync(request);
    }
}