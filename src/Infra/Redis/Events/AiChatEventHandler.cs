
namespace AJE.Infra.Redis.Events;

public class AiChatEventHandler : IAiChatEventHandler
{
    public Task SendAsync(AiChatEvent aiChatEvent)
    {
        // TODO
        throw new NotImplementedException();
    }

    public void Subscribe(Action<AiChatEvent> handler)
    {
        // TODO
        throw new NotImplementedException();
    }
}
