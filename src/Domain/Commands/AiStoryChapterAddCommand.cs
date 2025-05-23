
namespace AJE.Domain.Commands;

public record AiStoryChapterAddCommand : IRequest<AiStoryChapterAddedEvent>
{
    public Guid StoryId { get; init; }
    public string Title { get; init; } = string.Empty;
}

public class AiStoryChapterAddCommandHandler(
    IAiStoryRepository aiStoryRepository,
    IAiStoryEventHandler aiStoryEventHandler) : IRequestHandler<AiStoryChapterAddCommand, AiStoryChapterAddedEvent>
{
    private readonly IAiStoryRepository _aiStoryRepository = aiStoryRepository;
    private readonly IAiStoryEventHandler _aiStoryEventHandler = aiStoryEventHandler;

    public async Task<AiStoryChapterAddedEvent> Handle(AiStoryChapterAddCommand request, CancellationToken cancellationToken)
    {
        var chapterId = Guid.NewGuid();
        await _aiStoryRepository.AddChapterAsync(request.StoryId, chapterId, request.Title);
        var chapterAddedEvent = new AiStoryChapterAddedEvent
        {
            StoryId = request.StoryId,
            ChapterId = chapterId,
            Title = request.Title,
            EventTimeStamp = DateTimeOffset.UtcNow,
        };
        await _aiStoryEventHandler.SendAsync(chapterAddedEvent);
        return chapterAddedEvent;
    }
}