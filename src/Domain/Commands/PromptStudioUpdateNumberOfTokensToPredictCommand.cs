namespace AJE.Domain.Commands;

public record PromptStudioUpdateNumberOfTokensToPredictCommand : IRequest<PromptStudioNumberOfTokensToPredictUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required int NumberOfTokensToPredict { get; init; }
}

public class PromptStudioUpdateNumberOfTokensToPredictCommandHandler : IRequestHandler<PromptStudioUpdateNumberOfTokensToPredictCommand, PromptStudioNumberOfTokensToPredictUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateNumberOfTokensToPredictCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioNumberOfTokensToPredictUpdatedEvent> Handle(PromptStudioUpdateNumberOfTokensToPredictCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveNumberOfTokensToPredictAsync(command.SessionId, command.NumberOfTokensToPredict);
        var e = new PromptStudioNumberOfTokensToPredictUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            NumberOfTokensToPredict = command.NumberOfTokensToPredict
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}