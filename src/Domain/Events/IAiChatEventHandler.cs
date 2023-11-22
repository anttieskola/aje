namespace AJE.Domain.Events;

public interface IAiChatEventHandler
{
    Task SendAsync(AiChatEvent aiChatEvent);
}
