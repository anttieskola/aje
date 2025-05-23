namespace AJE.Domain.Commands;

public record AiStoryUpdateTitleCommand : IRequest<AiStoryTitleUpdatedEvent>
{
    public required Guid StoryId { get; init; }
    public required string Title { get; init; }
}

public class AiStoryUpdateTitleCommandHandler : IRequestHandler<AiStoryUpdateTitleCommand, AiStoryTitleUpdatedEvent>
{
    private readonly IAiStoryRepository _aiStoryRepository;
    private readonly IAiStoryEventHandler _aiStoryEventHandler;

    public AiStoryUpdateTitleCommandHandler(
        IAiStoryRepository aiStoryRepository,
        IAiStoryEventHandler aiStoryEventHandler)
    {
        _aiStoryRepository = aiStoryRepository;
        _aiStoryEventHandler = aiStoryEventHandler;
    }

    public async Task<AiStoryTitleUpdatedEvent> Handle(AiStoryUpdateTitleCommand command, CancellationToken cancellationToken)
    {
        await _aiStoryRepository.UpdateTitleAsync(command.StoryId, command.Title);
        var e = new AiStoryTitleUpdatedEvent
        {
            StoryId = command.StoryId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            Title = command.Title
        };
        await _aiStoryEventHandler.SendAsync(e);
        return e;
    }
}