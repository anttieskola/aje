using AJE.Infra.Redis;

namespace AJE.Test.Unit;
public class Playground
{
#pragma warning disable S2699
    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task FerrisWheel()
    {
        var id = AJE.Service.NewsDownloader.Yle.HtmlParser.CreateId("https://yle.fi/a/74-20025030");
        // {1fbf9390-2e7d-75ba-a9b1-4f8896754a0a}
        // zeus: article:1fbf9390-2e7d-75ba-a9b1-4f8896754a0a
        // ares: article:365e3ed8-416e-43be-9e1b-5c0836d4e7b7
        // ares: article:846510de-b512-4d74-8046-30a469379941
        var asQuery = RedisTokenizer.RedisEscape("https://yle.fi/a/74-20025030");
        // "@source:{https\\:\\/\\/yle\\.fi\\/a\\/74\\-20025030}"
        await Task.Delay(TimeSpan.FromMicroseconds(1));
    }
#pragma warning restore S2699
}
