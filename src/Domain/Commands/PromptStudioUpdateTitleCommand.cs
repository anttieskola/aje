namespace AJE.Domain.Commands;

public record PromptStudioUpdateTitleCommand : IRequest<PromptStudioTitleUpdatedEvent>
{
    public required Guid SessionId { get; init; }
    public required string Title { get; init; }
}

public class PromptStudioUpdateTitleCommandHandler : IRequestHandler<PromptStudioUpdateTitleCommand, PromptStudioTitleUpdatedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public PromptStudioUpdateTitleCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioTitleUpdatedEvent> Handle(PromptStudioUpdateTitleCommand command, CancellationToken cancellationToken)
    {
        await _promptStudioRepository.SaveTitleAsync(command.SessionId, command.Title);
        var e = new PromptStudioTitleUpdatedEvent
        {
            SessionId = command.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            Title = command.Title
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}