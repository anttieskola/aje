namespace AJE.Domain.Queries;

public record AiStoryGetQuery : IRequest<AiStory>
{
    public required Guid StoryId { get; init; }
}

public class AiStoryGetQueryHandler : IRequestHandler<AiStoryGetQuery, AiStory>
{
    private readonly IAiStoryRepository _repository;

    public AiStoryGetQueryHandler(IAiStoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiStory> Handle(AiStoryGetQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(request.StoryId);
    }
}