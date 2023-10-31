﻿using AJE.Domain.Commands;
using AJE.Domain.Events;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

public class ArticleRepositoryTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly IRedisIndex _index = new ArticleIndex();
    public ArticleRepositoryTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    private readonly Guid _id = new("00000000-0000-0000-0000-000000000010");
    private readonly string _source = "repository_integration_test";

    [Fact]
    public async Task GetBySourceAsync_OK()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_id.ToString()));
        var publishHandler = new AddArticleCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticle.Article_01(_id, _source);
        var publishEvent = await publishHandler.Handle(new AddArticleCommand { Article = article }, CancellationToken.None);
        Assert.NotNull(publishEvent);
        Assert.Equal(_id, publishEvent.Id);

        var repository = new ArticleRepository(
            new Mock<ILogger<ArticleRepository>>().Object,
             _redisFixture.Connection);

        // act
        var result = await repository.GetBySourceAsync(_source);
        Assert.NotNull(result);
        Assert.True(article == result);

        // clean
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_id.ToString()));
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