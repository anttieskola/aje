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
    /// TODO: Can this be done easier using aggregate?
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ScanLanguages()
    {
        using var connection = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
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
