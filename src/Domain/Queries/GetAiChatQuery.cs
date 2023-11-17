namespace AJE.Domain.Queries;

public record GetAiChatQuery : IRequest<AiChat>
{
    public required Guid Id { get; init; }
}

public class GetAiChatQueryHandler : IRequestHandler<GetAiChatQuery, AiChat>
{
    private readonly IAiChatRepository _repository;

    public GetAiChatQueryHandler(IAiChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiChat> Handle(GetAiChatQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.Id);
    }
}