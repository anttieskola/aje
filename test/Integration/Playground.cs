using AJE.Infra.Redis.Indexes;

namespace AJE.Test.Integration;

public class Playground
{
#pragma warning disable S2699
    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task Rollercoaster()
    {
        // Code is commented because I need it later for development.
        //var content = await File.ReadAllTextAsync("/var/aje/yle/74-20063011.html");
        //var article = HtmlParser.Parse(content, Guid.NewGuid());
        await Task.Delay(TimeSpan.FromMicroseconds(1));
    }

    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task HauntedHouse()
    {
        await Task.Delay(TimeSpan.FromMicroseconds(1));
    }
#pragma warning restore S2699

    /// <summary>
    /// This will fail when no articles in redis. Can be ignored.
    /// Used to scan language codes for UI.
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "This will fail when no articles in redis. Can be ignored. Used to scan language codes for UI.")]
    public async Task ScanLanguages()
    {
        using var connection = await ConnectionMultiplexer.ConnectAsync(TestConstants.RedisAddress);
        var db = connection.GetDatabase();
        var index = new ArticleIndex();
        var arguments = new string[] { index.Name, "*", "RETURN", "1", "$.language", "LIMIT", "0", "10000" };
        var result = await db.ExecuteAsync("FT.SEARCH", arguments);
        var rows = (RedisResult[])result!;
        var languages = new List<string>();
        for (long i = 1; i < rows.LongLength; i += 2)
        {
            var data = (RedisResult[])rows[i + 1]!;
            var language = (string)data[1]!;
            if (!languages.Contains(language))
                languages.Add(language);
        }
        languages.Sort();
        Assert.NotEmpty(languages);
    }
}
