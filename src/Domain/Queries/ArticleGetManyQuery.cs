﻿namespace AJE.Domain.Queries;

public record ArticleGetManyQuery : PaginatedQuery, IRequest<PaginatedList<Article>>
{
    public ArticleCategory? Category { get; init; }
    public string? Language { get; init; }
    public Polarity? Polarity { get; init; }
    public int? MaxPolarityVersion { get; init; }
    public bool? IsValidated { get; init; }
    public int? MaxTokenCount { get; init; }
    public bool? IsLiveNews { get; init; }
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