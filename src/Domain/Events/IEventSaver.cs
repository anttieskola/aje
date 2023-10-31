namespace AJE.Domain.Events;

public interface IEventSaver
{
    Task SaveAsync(ArticleAddedEvent articleAddedEvent, CancellationToken cancellationToken);
    Task SaveAsync(ArticleUpdatedEvent articleUpdatedEvent, CancellationToken cancellationToken);
}
