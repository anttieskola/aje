namespace AJE.Domain.Commands;

public class AiStoryStartCommand : IRequest<AiStoryEvent>
{
    public required Guid StoryId { get; init; }
}

public class AiStoryStartCommandHandler(
    IAiStoryRepository aiStoryRepository,
    IAiStoryEventHandler aiStoryEventHandler) : IRequestHandler<AiStoryStartCommand, AiStoryEvent>
{
    private readonly IAiStoryRepository _aiStoryRepository = aiStoryRepository;
    private readonly IAiStoryEventHandler _aiStoryEventHandler = aiStoryEventHandler;

    public async Task<AiStoryEvent> Handle(AiStoryStartCommand command, CancellationToken cancellationToken)
    {
        var story = await _aiStoryRepository.AddAsync(command.StoryId);
        var startedEvent = new AiStoryStartedEvent
        {
            StoryId = story.StoryId,
            EventTimeStamp = DateTimeOffset.UtcNow,
        };
        await _aiStoryEventHandler.SendAsync(startedEvent);
        return startedEvent;
    }
}
