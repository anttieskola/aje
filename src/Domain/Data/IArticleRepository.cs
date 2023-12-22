namespace AJE.Domain.Data;

public interface IArticleRepository
{
    Task AddAsync(Article article);
    Task UpdateAsync(Article article);
    Task UpdatePolarityAsync(Guid id, int polarityVersion, Polarity polarity);
    Task UpdateSummaryAsync(Guid id, int summaryVersion, string summary);
    Task UpdatePositiveThingsAsync(Guid id, int positiveThingsVersion, string positiveThings);
    Task<Article> GetAsync(Guid id);
    Task<PaginatedList<Article>> GetAsync(ArticleGetManyQuery query);
    Task<Article> GetBySourceAsync(string source);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleGetHeadersQuery query);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleSearchHeadersQuery query);
    Task<bool> ExistsAsync(string source);
}
