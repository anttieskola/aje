namespace AJE.Domain.Data;

public interface IArticleRepository
{
    Task AddAsync(Article article);
    Task UpdateAsync(Article article);
    Task UpdateIsValidatedAsync(Guid id, bool isValidated);
    Task UpdateTokenCountAsync(Guid id, int tokenCount);
    Task<Article> GetAsync(Guid id);
    Task<PaginatedList<Article>> GetAsync(ArticleGetManyQuery query);
    Task<Article> GetBySourceAsync(string source);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleGetHeadersQuery query);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(ArticleSearchHeadersQuery query);
    Task<bool> ExistsAsync(string source);
}
