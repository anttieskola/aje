using AJE.Domain.Commands;
using AJE.Domain.Entities;
using AJE.Domain.Enums;
using AJE.Domain.Events;
using AJE.Domain.Queries;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

/// <summary>
/// Tests require redis running on localhost:6379
/// </summary>
public class ArticleRepositoryTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly IRedisIndex _index = new ArticleIndex();
    public ArticleRepositoryTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    private readonly Guid _idForOk = new("00000000-0000-0000-0000-000000000010");
    private Article TestArticleForOk()
    {
        return new Article
        {
            Id = _idForOk,
            Category = ArticleCategory.BOGUS,
            Title = "test article for get by source",
            Modified = DateTimeOffset.UtcNow.AddYears(-110).Ticks,
            Published = true,
            Source = "repository_integration_test_get_by_source",
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1,
            IsValidated = true,
        };
    }
    [Fact]
    public async Task GetBySourceAsync_OK()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
        var publishHandler = new AddArticleCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticleForOk();
        var publishEvent = await publishHandler.Handle(new AddArticleCommand { Article = article }, CancellationToken.None);
        Assert.NotNull(publishEvent);
        Assert.Equal(_idForOk, publishEvent.Id);

        var repository = new ArticleRepository(
            new Mock<ILogger<ArticleRepository>>().Object,
             _redisFixture.Connection);

        // act
        var result = await repository.GetBySourceAsync("repository_integration_test_get_by_source");
        Assert.NotNull(result);
        Assert.True(article == result);

        // clean
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
    }

    private readonly Guid _idForIsValidated = new("00000000-0000-0000-0000-000000000011");
    private Article TestArticleForIsValidated()
    {
        return new Article
        {
            Id = _idForIsValidated,
            Category = ArticleCategory.BOGUS,
            Title = "test article for IsValidated",
            Modified = DateTimeOffset.UtcNow.AddYears(-110).Ticks,
            Published = true,
            Source = "integration_test_get_by_isvalidated",
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1,
            IsValidated = false,
        };
    }
    [Fact]
    public async Task IsValidated()
    {
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForIsValidated.ToString()));
        var publishHandler = new AddArticleCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticleForIsValidated();
        var publishEvent = await publishHandler.Handle(new AddArticleCommand { Article = article }, CancellationToken.None);
        Assert.NotNull(publishEvent);
        Assert.Equal(_idForIsValidated, publishEvent.Id);

        var repository = new ArticleRepository(
            new Mock<ILogger<ArticleRepository>>().Object,
             _redisFixture.Connection);

        // act (test run paraller so there can be many invalid articles)
        var result = await repository.GetAsync(new GetArticlesQuery
        {
            Category = ArticleCategory.BOGUS,
            IsValidated = false,
            Offset = 0,
            PageSize = 100,
        });
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        var articleCopy = result.Items.Single(i => i.Id == _idForIsValidated);
        Assert.True(article == articleCopy);

        await repository.UpdateIsValidated(_idForIsValidated, true);
        result = await repository.GetAsync(new GetArticlesQuery
        {
            Category = ArticleCategory.BOGUS,
            IsValidated = false,
            Offset = 0,
            PageSize = 100,
        });
        Assert.NotNull(result);
        var notExistingCopy = result.Items.SingleOrDefault(i => i.Id == _idForIsValidated);
        Assert.Null(notExistingCopy);

        // clean
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForIsValidated.ToString()));
    }

    [Fact]
    public async Task GetBySourceAsync_NOTFOUND()
    {
        var repository = new ArticleRepository(
            new Mock<ILogger<ArticleRepository>>().Object,
             _redisFixture.Connection);

        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repository.GetBySourceAsync("repository_integration_test_that_does_not_exist"));
    }
}
