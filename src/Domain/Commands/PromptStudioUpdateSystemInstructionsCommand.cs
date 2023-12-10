namespace AJE.Domain.Commands;

public record PromptStudioUpdateSystemInstructionsCommand : IRequest<PropmtStudioSystemInstructionsUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required EquatableList<string> SystemInstructions { get; init; }
}

public class PromptStudioUpdateSystemInstructionsCommandHandler : IRequestHandler<PromptStudioUpdateSystemInstructionsCommand, PropmtStudioSystemInstructionsUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateSystemInstructionsCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PropmtStudioSystemInstructionsUpdatedEvent> Handle(PromptStudioUpdateSystemInstructionsCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveSystemInstructionsAsync(command.SessionId, command.SystemInstructions);
        var e = new PropmtStudioSystemInstructionsUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            SystemInstructions = command.SystemInstructions
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}
