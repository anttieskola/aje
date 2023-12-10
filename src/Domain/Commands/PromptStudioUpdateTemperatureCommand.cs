namespace AJE.Domain.Commands;

public record PromptStudioUpdateTemperatureCommand : IRequest<PromptStudioTemperatureUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required double Temperature { get; init; }
}

public class PromptStudioUpdateTemperatureCommandHandler : IRequestHandler<PromptStudioUpdateTemperatureCommand, PromptStudioTemperatureUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateTemperatureCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioTemperatureUpdatedEvent> Handle(PromptStudioUpdateTemperatureCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveTemperatureAsync(command.SessionId, command.Temperature);
        var e = new PromptStudioTemperatureUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            Temperature = command.Temperature
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}