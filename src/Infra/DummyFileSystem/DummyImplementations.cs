
namespace AJE.Infra.DummyFileSystem;

public class DummyYleRepository : IYleRepository
{
    public Task<string> GetHtmlAsync(Uri uri, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Uri[]> GetUriList()
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync(Uri uri, string html, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Uri uri, string html, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}