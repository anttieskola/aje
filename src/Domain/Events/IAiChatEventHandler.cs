namespace AJE.Domain.Events;

public interface IAiChatEventHandler
{
    Task SendAsync(AiChatEvent aiChatEvent);
    void Subscribe(Action<AiChatEvent> handler);
}
