namespace AJE.Domain.Events;

public interface IPromptStudioEventHandler
{
    Task SendAsync(PromptStudioEvent promptStudioEvent);
}
