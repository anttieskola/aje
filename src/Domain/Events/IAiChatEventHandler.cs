namespace AJE.Domain.Events;

public interface IAiChatEventHandler
{
    Task SendAsync(AiChatEvent aiChatEvent);
    void Subscribe(Guid subscriberId, Func<AiChatEvent, Task> handler);
    void SubscribeToChat(Guid subscriberId, Guid chatId, Func<AiChatEvent, Task> handler);
    void Unsubscribe(Guid subscriberId);
}
