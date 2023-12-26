namespace AJE.Domain.Queries;

public record ArticleGetManyQuery : PaginatedQuery, IRequest<PaginatedList<Article>>
{
    public ArticleCategory? Category { get; init; }
    public string? Language { get; init; }
    public string[]? Languages { get; init; }
    public Polarity? Polarity { get; init; }
    public bool? IsLiveNews { get; init; }
    public bool? IsValidForAnalysis { get; init; }
    public int? MaxPolarityVersion { get; init; }
    public int? MaxSummaryVersion { get; init; }
    public int? MaxPositiveThingsVersion { get; init; }
    public int? MaxLocationsVersion { get; init; }
    public int? MaxOrganizationsVersion { get; init; }
    public int? MaxCorporationsVersion { get; init; }
    public int? MaxKeyPeopleVersion { get; init; }
}

public class ArticleGetManyQueryHandler : IRequestHandler<ArticleGetManyQuery, PaginatedList<Article>>
{
    private readonly IArticleRepository _repository;

    public ArticleGetManyQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<Article>> Handle(ArticleGetManyQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request);
    }
}