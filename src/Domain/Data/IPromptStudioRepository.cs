namespace AJE.Domain.Data;

public interface IPromptStudioRepository
{
    Task<PromptStudioSession> AddAsync(PromptStudioOptions options);
    Task<PromptStudioSession> AddRunAsync(Guid sessionId, PromptStudioRun run);
    Task<PromptStudioSession> GetAsync(Guid sessionId);
    Task<PaginatedList<PromptStudioSessionHeader>> GetHeadersAsync(PromptStudioGetManySessionHeadersQuery query);
    Task SaveTitleAsync(Guid sessionId, string title);
    Task SaveTemperatureAsync(Guid sessionId, double temperature);
    Task SaveNumberOfTokensEvaluatedAsync(Guid sessionId, int numberOfTokensEvaluated);
    Task SaveEntityNameAsync(Guid sessionId, string entityName);
    Task SaveSystemInstructionsAsync(Guid sessionId, EquatableList<string> systemInstructions);
    Task SaveContextAsync(Guid sessionId, string context);

}
