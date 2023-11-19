namespace AJE.Domain.Events;

public interface IAiChatEventHandler
{
    Task SendAsync(AiChatEvent aiChatEvent);
    void Subscribe(Action<AiChatEvent> handler);
    void Subscribe(Guid chatId, Action<AiChatEvent> handler);
    void Unsubscribe(Action<AiChatEvent> handler);
}
