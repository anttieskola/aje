namespace AJE.Domain.Commands;

public record PromptStudioUpdateNumberOfTokensEvaluatedCommand : IRequest<PromptStudioNumberOfTokensEvaluatedUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required int NumberOfTokensEvaluated { get; init; }
}

public class PromptStudioUpdateNumberOfTokensEvaluatedCommandHandler : IRequestHandler<PromptStudioUpdateNumberOfTokensEvaluatedCommand, PromptStudioNumberOfTokensEvaluatedUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateNumberOfTokensEvaluatedCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioNumberOfTokensEvaluatedUpdatedEvent> Handle(PromptStudioUpdateNumberOfTokensEvaluatedCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveNumberOfTokensEvaluatedAsync(command.SessionId, command.NumberOfTokensEvaluated);
        var e = new PromptStudioNumberOfTokensEvaluatedUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            NumberOfTokensEvaluated = command.NumberOfTokensEvaluated
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}