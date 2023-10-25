namespace AJE.Domain.Queries;

public record GetArticleQuery : IRequest<Article>
{
    public required Guid Id { get; init; }
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
        return await _repository.GetAsync(request.Id);
    }
}
