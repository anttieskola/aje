
namespace AJE.Domain.Queries;

#region ById

public record GetArticleByIdQuery : IRequest<Article>
{
    public required Guid Id { get; init; }
}

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Article>
{
    private readonly IArticleRepository _repository;

    public GetArticleByIdQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.Id);
    }
}

#endregion ById

#region BySource

public record GetArticleBySourceQuery : IRequest<Article>
{
    public required string Source { get; init; }
}

public class GetArticleBySourceQueryHandler : IRequestHandler<GetArticleBySourceQuery, Article>
{
    private readonly IArticleRepository _repository;

    public GetArticleBySourceQueryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Article> Handle(GetArticleBySourceQuery request, CancellationToken cancellationToken)
    {

        return await _repository.GetBySourceAsync(request.Source);
    }
}

#endregion BySource