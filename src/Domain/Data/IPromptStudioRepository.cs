namespace AJE.Domain.Data;

public interface IPromptStudioRepository
{
    Task<PromptStudioSession> AddAsync(PromptStudioOptions options);
    Task<PromptStudioSession> AddRunAsync(Guid sessionId, PromptStudioSessionRun run);
    Task<PromptStudioSession> GetAsync(Guid sessionId);
}
