namespace AJE.Domain.Commands;

public record StartPromptStudioCommand : IRequest<PromptStudioStartEvent>
{
    public bool IsTest { get; init; } = false;
    public required Guid SessionId { get; init; }
}

public class StartPromptStudioCommandHandler : IRequestHandler<StartPromptStudioCommand, PromptStudioStartEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;

    public StartPromptStudioCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
    }

    public async Task<PromptStudioStartEvent> Handle(StartPromptStudioCommand request, CancellationToken cancellationToken)
    {
        var options = new PromptStudioOptions
        {
            SessionId = request.SessionId,
        };
        var session = await _promptStudioRepository.AddAsync(options);
        var e = new PromptStudioStartEvent
        {
            IsTest = request.IsTest,
            SessionId = session.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
        };
        await _promptStudioEventHandler.SendAsync(e);
        return e;
    }
}