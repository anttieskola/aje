using AJE.Infra.Redis.Indexes;

namespace AJE.Test.Integration;

/// <summary>
/// Test requires Redis server running
/// </summary>
public class ArticleIndexTest : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly ArticleIndex _index = new();
    public ArticleIndexTest(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    /// <summary>
    /// This test only works after only articles loaded
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "Only works after articles loaded")]
    public async Task TestIndexAfterArticleReload()
    {
        var db = _redisFixture.Connection.GetDatabase();
        var infoResult = await db.ExecuteAsync("FT.INFO", _index.Name);
        var infoRows = (RedisResult[])infoResult!;
        //  9. number of documents in the index
        var documents = (long)infoRows[9];
        // 45. is still indexing?
        var isIndexing = (bool)infoRows[45];
        // tokenCount
        {
            var result = await db.ExecuteAsync("FT.SEARCH", _index.Name, "@tokenCount:[-inf -1]", "LIMIT", "0", "0");
            var rows = (RedisResult[])result!;
            var totalCount = (long)rows[0];
            Assert.Equal(documents, totalCount);
        }

        // polarity
        {
            var result = await db.ExecuteAsync("FT.SEARCH", _index.Name, "@polarityVersion:[-inf 0]", "LIMIT", "0", "0");
            var rows = (RedisResult[])result!;
            var totalCount = (long)rows[0];
            Assert.Equal(documents, totalCount);
        }

    }
}
