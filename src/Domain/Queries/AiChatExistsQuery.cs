
namespace AJE.Domain.Queries;

public record AiChatExistsQuery : IRequest<bool>
{
    public required Guid Id { get; init; }
}

public class AiChatExistsQueryHandler : IRequestHandler<AiChatExistsQuery, bool>
{
    private readonly IAiChatRepository _repository;

    public AiChatExistsQueryHandler(IAiChatRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(AiChatExistsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}