namespace AJE.Domain.Data;

public interface IYleRepository
{
    /// <summary>
    /// Store loaded html to repository
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    Task StoreAsync(Uri uri, string html);

    /// <summary>
    /// Update stored html content
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    Task UpdateAsync(Uri uri, string html);

    /// <summary>
    /// Get list of stored html content's uri's
    /// </summary>
    /// <returns></returns>
    Task<Uri[]> GetUriList();

    /// <summary>
    /// Get single uri's html content
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    Task<string> GetHtmlAsync(Uri uri);

}
