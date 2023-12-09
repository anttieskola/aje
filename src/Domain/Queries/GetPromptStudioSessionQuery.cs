namespace AJE.Domain.Queries;

public class GetPromptStudioSessionQuery : IRequest<PromptStudioSession>
{
    public required Guid SessionId { get; init; }
}

public class GetPromptStudioSessionQueryHandler : IRequestHandler<GetPromptStudioSessionQuery, PromptStudioSession>
{
    private readonly IPromptStudioRepository _repository;

    public GetPromptStudioSessionQueryHandler(IPromptStudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PromptStudioSession> Handle(GetPromptStudioSessionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.SessionId);
    }
}
