namespace AJE.Domain.Commands;

public record PromptStudioUpdateContextCommand : IRequest<PromptStudioContextUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required string Context { get; init; }
}

public class PromptStudioUpdateContextCommandHandler : IRequestHandler<PromptStudioUpdateContextCommand, PromptStudioContextUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateContextCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioContextUpdatedEvent> Handle(PromptStudioUpdateContextCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveContextAsync(command.SessionId, command.Context);
        var e = new PromptStudioContextUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            Context = command.Context
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}