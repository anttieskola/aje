namespace AJE.Domain.Data;

public interface IPromptStudioRepository
{
    Task<PromptStudioSession> AddAsync(PromptStudioOptions options);
    Task<PromptStudioSession> AddRunAsync(Guid sessionId, PromptStudioRun run);
    Task<PromptStudioSession> GetAsync(Guid sessionId);
}
