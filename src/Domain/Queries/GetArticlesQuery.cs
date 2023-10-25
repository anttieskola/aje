namespace AJE.Domain.Queries;

public record GetArticlesQuery : PaginatedQuery, IRequest<PaginatedList<Article>>
{
    public Category? Category { get; init; }
    public bool? Published { get; init; }
    public string? Language { get; init; }
    public Polarity? Polarity { get; init; }
    public int? MaxPolarityVersion { get; init; }
}

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, PaginatedList<Article>>
{
    private readonly IArticleRepository _repository;

    public GetArticlesQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<Article>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request);
    }
}