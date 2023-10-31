using AJE.Domain.Commands;
using AJE.Domain.Entities;
using AJE.Domain.Events;
using AJE.Domain.Queries;
using AJE.Infra.Redis.Indexes;

namespace AJE.Test.Integration.Application;

/// <summary>
/// Tests require redis running on localhost:6379
/// </summary>
public class ArticleTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly IRedisIndex _index = new ArticleIndex();
    public ArticleTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    private readonly Guid _idOk = new("00000000-0000-0000-0000-000000000001");
    private readonly Guid _idMissing = new("00000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task Lifecycle()
    {
        // arrange article
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idOk.ToString()));
        var source = "https://www.anttieskola.com";
        var publishHandler = new AddArticleCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticle.Article_01(_idOk, source);

        // act: publish article
        var publishEvent = await publishHandler.Handle(new AddArticleCommand { Article = article }, CancellationToken.None);
        Assert.NotNull(publishEvent);
        Assert.Equal(_idOk, publishEvent.Id);

        // act: query that article exists
        var existsHandler = new ArticleExistsQueryHandler(_redisFixture.ArticleRepository);
        var exists = await existsHandler.Handle(new ArticleExistsQuery { Source = source }, CancellationToken.None);
        Assert.True(exists);

        // act: get article
        var articleHandler = new GetArticleQueryHandler(_redisFixture.ArticleRepository);
        var copy = await articleHandler.Handle(new GetArticleQuery { Id = _idOk }, CancellationToken.None);

        // check article
        Assert.NotNull(copy);
        Assert.Equal(article.Id, copy.Id);
        Assert.Equal(article.Category, copy.Category);
        Assert.Equal(article.Title, copy.Title);
        Assert.Equal(article.Modified, copy.Modified);
        Assert.Equal(article.Published, copy.Published);
        Assert.Equal(article.Source, copy.Source);
        Assert.Equal(article.Language, copy.Language);
        Assert.Equal(article.Content.Count(), copy.Content.Count());
        {
            var h1 = copy.Content.ElementAt(0) as MarkdownHeaderElement;
            Assert.NotNull(h1);
            Assert.Equal(1, h1.Level);
            Assert.Equal("This is header 1", h1.Text);

            var p1 = copy.Content.ElementAt(1) as MarkdownTextElement;
            Assert.NotNull(p1);
            Assert.Equal("This is a paragraph", p1.Text);

            var h2 = copy.Content.ElementAt(2) as MarkdownHeaderElement;
            Assert.NotNull(h2);
            Assert.Equal(2, h2.Level);
            Assert.Equal("This is header 2", h2.Text);

            var p2 = copy.Content.ElementAt(3) as MarkdownTextElement;
            Assert.NotNull(p2);
            Assert.Equal("This is another paragraph", p2.Text);
        }

        // act: get article headers (paged)
        var headersHandler = new GetArticleHeadersQueryHandler(_redisFixture.ArticleRepository);
        var headers = await headersHandler.Handle(new GetArticleHeadersQuery { Offset = 0, PageSize = 10 }, CancellationToken.None);
        Assert.NotNull(headers);
        Assert.NotEmpty(headers.Items);

        // act: get articles (paged)
        var articlesHandler = new GetArticlesQueryHandler(_redisFixture.ArticleRepository);
        var articles = await articlesHandler.Handle(new GetArticlesQuery { Offset = 0, PageSize = 10 }, CancellationToken.None);
        Assert.NotNull(articles);
        Assert.NotEmpty(articles.Items);

        // clean
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idOk.ToString()));
    }


    [Fact]
    public async Task GetArticleQuery_Missing()
    {
        // arrange
        var getHandler = new GetArticleQueryHandler(_redisFixture.ArticleRepository);
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idMissing.ToString()));

        // act
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await getHandler.Handle(new GetArticleQuery { Id = _idMissing }, CancellationToken.None);
        });
    }
}
