namespace AJE.Domain.Queries;

public class PromptStudioGetSessionQuery : IRequest<PromptStudioSession>
{
    public required Guid SessionId { get; init; }
}

public class PromptStudioGetSessionQueryHandler : IRequestHandler<PromptStudioGetSessionQuery, PromptStudioSession>
{
    private readonly IPromptStudioRepository _repository;

    public PromptStudioGetSessionQueryHandler(IPromptStudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PromptStudioSession> Handle(PromptStudioGetSessionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.SessionId);
    }
}
