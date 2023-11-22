namespace AJE.Domain.Events;

public interface IArticleEventHandler
{
    Task SendAsync(ArticleEvent articleEvent);
}
