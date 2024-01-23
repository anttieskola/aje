namespace AJE.Infra.Database;

public class AiStoryRepository : IAiStoryRepository
{
    public Task<AiStory> AddAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddChapterAsync(Guid storyId, Guid chapterId, string title)
    {
        throw new NotImplementedException();
    }

    public Task<AiStory> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTitleAsync(Guid id, string title)
    {
        throw new NotImplementedException();
    }
}
