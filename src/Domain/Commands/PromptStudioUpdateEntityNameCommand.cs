
namespace AJE.Domain.Commands;

public record PromptStudioUpdateEntityNameCommand : IRequest<PromptStudioEntityNameUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required string EntityName { get; init; }
}

public class PromptStudioUpdateEntityNameCommandHandler : IRequestHandler<PromptStudioUpdateEntityNameCommand, PromptStudioEntityNameUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateEntityNameCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioEntityNameUpdatedEvent> Handle(PromptStudioUpdateEntityNameCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveEntityNameAsync(command.SessionId, command.EntityName);
        var e = new PromptStudioEntityNameUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            EntityName = command.EntityName
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}