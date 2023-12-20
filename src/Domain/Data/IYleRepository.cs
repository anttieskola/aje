namespace AJE.Domain.Data;

public interface IYleRepository
{
    /// <summary>
    /// Store loaded html to repository
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="html"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StoreAsync(Uri uri, string html, CancellationToken cancellationToken);

    /// <summary>
    /// Update stored html content
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="html"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(Uri uri, string html, CancellationToken cancellationToken);


    /// <summary>
    /// Check if html content exists in repository
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// Get list of stored html content's uri's
    /// </summary>
    /// <returns></returns>
    Task<Uri[]> GetUriList();

    /// <summary>
    /// Get single uri's html content
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetHtmlAsync(Uri uri, CancellationToken cancellationToken);

}
