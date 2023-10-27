using AJE.Infra.Indexes;

namespace AJE.Test.Integration;

public class Playground
{

    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task Rollercoaster()
    {
        await Task.Delay(TimeSpan.FromMicroseconds(1));
        // using var connection = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        // var db = connection.GetDatabase();
        // var repository = new ArticleRepository(new Mock<ILogger<ArticleRepository>>().Object, connection);
        // var contextCreator = new ArticleContextCreator(new MarkDownSimplifier());

        // var guid = new Guid("7d5844ce-825a-4c39-a933-587be0d3459a");
        // var article = await repository.GetAsync(guid);
        // var context = contextCreator.Create(article);
    }

    /// <summary>
    /// Ready, set and go. Playground for new stuff.
    /// </summary>
    [Fact]
    public async Task HauntedHouse()
    {
        await Task.Delay(TimeSpan.FromMicroseconds(1));
        // so big article
        // var html = await File.ReadAllTextAsync("/var/aje/yle/74-20052908.html");
        // var article = HtmlParser.Parse(html);
        // var contextCreator = new ArticleContextCreator(new MarkDownSimplifier());
        // var context = contextCreator.Create(article);
    }

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
        var totalCount = (long)rows[0];
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
