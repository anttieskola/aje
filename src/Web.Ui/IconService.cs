namespace AJE.Web.Ui
{
    /// <summary>
    /// Provides SVG icons for the app
    /// </summary>
    public interface IIconService
    {
        Task<List<string>> List();
        Task<string> Icon(string Name);
    }

    public class IconService: IIconService
    {
        private static readonly string ICON_PREFIX = "ICON:";
        private static readonly string ICON_LIST = "ICON_NAMES";
        private static readonly string LOADING = "ICONS_LOADING";
        private readonly IMemoryCache _memoryCache;

        public IconService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<List<string>> List()
        {
            if (_memoryCache.TryGetValue(ICON_LIST, out List<string>? icons))
            {
                if (icons != null)
                    return icons;
            }
            await LoadIcons();
            if (_memoryCache.TryGetValue(ICON_LIST, out icons))
            {
                if (icons != null)
                    return icons;
            }
            throw new Exception("Error loading icons");
        }

        public async Task<string> Icon(string name)
        {
            if (_memoryCache.TryGetValue($"{ICON_PREFIX}{name}", out string? svg))
            {
                if (svg != null)
                    return svg;
            }
            await LoadIcons();
            if (_memoryCache.TryGetValue($"{ICON_PREFIX}{name}", out svg))
            {
                if (svg != null)
                    return svg;
            }
            throw new Exception("Error loading icons");
        }
        
        public async Task LoadIcons()
        {
            if (_memoryCache.TryGetValue(ICON_LIST, out List<string>? icons))
            {
                if (icons != null)
                    return;
            }

            if (_memoryCache.TryGetValue(LOADING, out bool loading))
            {
                if (loading)
                    return;
            }

            _memoryCache.Set(LOADING, true);
            // read file from disk into byte array
            var file = await File.ReadAllBytesAsync("icons.zip");
            var names = new List<string>();
            // use unzipp on the byte array
            using (var memoryStream = new MemoryStream(file))
            {
                using (var archive = new ZipArchive(memoryStream))
                {
                    foreach (var iconFile in archive.Entries)
                    {
                        if (iconFile.Length == 0)
                            continue;

                        var name = iconFile.Name.Replace(".svg", string.Empty);
                        names.Add(name);
                        using var inputStream = iconFile.Open();
                        using var reader = new StreamReader(inputStream, Encoding.UTF8);
                        var svg = await reader.ReadToEndAsync();
                        _memoryCache.Set($"{ICON_PREFIX}{name}", svg);
                    }
                }
            }
            _memoryCache.Set(ICON_LIST, names.Order().ToList());
            _memoryCache.Set(LOADING, false);
        }
    }
}
