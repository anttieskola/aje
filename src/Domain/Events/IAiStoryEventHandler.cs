namespace AJE.Domain.Events;

public interface IAiStoryEventHandler
{
    Task SendAsync(AiStoryEvent aiStoryEvent);
}
