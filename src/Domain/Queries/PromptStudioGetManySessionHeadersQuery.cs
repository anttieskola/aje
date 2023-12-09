namespace AJE.Domain.Queries;

public record PromptStudioGetManySessionHeadersQuery : PaginatedQuery, IRequest<PaginatedList<PromptStudioSessionHeader>>
{
}

public class PromptStudioGetManySessionHeadersQueryHandler : IRequestHandler<PromptStudioGetManySessionHeadersQuery, PaginatedList<PromptStudioSessionHeader>>
{
    private readonly IPromptStudioRepository _repository;

    public PromptStudioGetManySessionHeadersQueryHandler(IPromptStudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<PromptStudioSessionHeader>> Handle(PromptStudioGetManySessionHeadersQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetHeadersAsync(query);
    }
}