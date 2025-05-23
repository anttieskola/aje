namespace AJE.Domain.Data;

public interface IAiStoryRepository
{
    Task<AiStory> AddAsync(Guid id);
    Task<AiStory> GetAsync(Guid id);

    Task UpdateTitleAsync(Guid id, string title);

    Task AddChapterAsync(Guid storyId, Guid chapterId, string title);
}
