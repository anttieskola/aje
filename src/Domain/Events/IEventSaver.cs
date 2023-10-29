namespace AJE.Domain.Events;

public interface IEventSaver
{
    Task SaveAsync(ArticleClassifiedEvent articleClassifiedEvent, CancellationToken cancellationToken);
}
