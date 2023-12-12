namespace AJE.Infra.FileSystem;

public class YleRepository : IYleRepository
{
    private readonly FileSystemConfiguration _configuration;
    private readonly string _yleFolder;

    public YleRepository(
        FileSystemConfiguration configuration)
    {
        _configuration = configuration;
        _yleFolder = Path.Combine(_configuration.RootFolder, "yle");
    }

    private static string GetFileName(Uri uri)
    {
        var fileName = uri.ToString().Replace("https://yle.fi/a/", string.Empty);
        return $"{fileName}.html";
    }

    public Task<string> GetHtmlAsync(Uri uri)
    {
        var fileWithPath = Path.Combine(_yleFolder, GetFileName(uri));
        return File.ReadAllTextAsync(fileWithPath);
    }

    public Task<Uri[]> GetUriList()
    {
        var files = Directory.GetFiles(_yleFolder, "*.html");
        var uris = new List<Uri>();
        foreach (var file in files)
        {
            uris.Add(new Uri($"https://yle.fi/a/{Path.GetFileNameWithoutExtension(file)}"));
        }
        return Task.FromResult(uris.ToArray());
    }

    public async Task StoreAsync(Uri uri, string html)
    {
        var fileName = Path.Combine(_yleFolder, GetFileName(uri));
        if (File.Exists(fileName))
        {
            throw new PlatformException($"{fileName} already exists");
        }
        else
        {
            await File.WriteAllTextAsync(fileName, html);
        }
    }

    public async Task UpdateAsync(Uri uri, string html)
    {
        var fileName = Path.Combine(_yleFolder, GetFileName(uri));
        if (!File.Exists(fileName))
        {
            throw new PlatformException($"{fileName} does not exist");
        }
        else
        {
            await File.WriteAllTextAsync(fileName, html);
        }
    }
}
