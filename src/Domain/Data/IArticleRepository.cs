namespace AJE.Domain.Data;

public interface IArticleRepository
{
    Task AddAsync(Article article);
    Task<Article> GetAsync(Guid id);
    Task<PaginatedList<Article>> GetAsync(GetArticlesQuery query);
    Task<PaginatedList<ArticleHeader>> GetHeadersAsync(GetArticleHeadersQuery query);
    Task<bool> ExistsAsync(string source);
}
