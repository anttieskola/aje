namespace AJE.Domain.Data;

public interface IArticleRepository
{
    Task AddAsync(Article article);
    Task UpdateAsync(Article article);
    Task UpdateIsValidated(Guid id, bool isValidated);
    Task<Article> GetAsync(Guid id);
    Task<PaginatedList<Article>> GetAsync(GetArticlesQuery query);
    Task<Article> GetBySourceAsync(string source);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(GetArticleHeadersQuery query);
    Task<bool> ExistsAsync(string source);
}
