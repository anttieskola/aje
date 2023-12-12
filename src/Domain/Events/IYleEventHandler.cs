namespace AJE.Domain.Events;

public interface IYleEventHandler
{
    Task SendAsync(YleEvent yleEvent);
}
