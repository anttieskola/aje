namespace AJE.Domain.Queries;

public record AiChatGetQuery : IRequest<AiChat>
{
    public required Guid Id { get; init; }
}

public class AiChatGetQueryHandler : IRequestHandler<AiChatGetQuery, AiChat>
{
    private readonly IAiChatRepository _repository;

    public AiChatGetQueryHandler(IAiChatRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiChat> Handle(AiChatGetQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.Id);
    }
}