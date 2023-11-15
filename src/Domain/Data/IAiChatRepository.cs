
namespace AJE.Domain.Data;

public interface IAiChatRepository
{
    Task<AiChat> AddAsync(AiChatOptions options);
    Task<AiChat> GetAsync(Guid id);
    Task<AiChat> AddHistoryEntry(Guid chatId, AiChatInteractionEntry entry);
}
