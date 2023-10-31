namespace AJE.Domain.Queries;

public record GetArticleQuery : IRequest<Article>
{
    public Guid? Id { get; init; }
    public string? Source { get; init; }
}

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, Article>
{
    private readonly IArticleRepository _repository;

    public GetArticleQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
            return await _repository.GetAsync(request.Id.Value);
        else if (!string.IsNullOrEmpty(request.Source))
            return await _repository.GetBySourceAsync(request.Source);
        else
            throw new ArgumentNullException($"{nameof(request.Id)} or {nameof(request.Source)} must be provided");
    }
}
