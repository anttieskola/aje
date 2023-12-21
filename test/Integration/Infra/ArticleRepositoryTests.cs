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
/// Tests require redis running
/// </summary>
public class ArticleRepositoryTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly ArticleIndex _index = new();
    public ArticleRepositoryTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    [Fact]
    private async Task SearchMultipleLanguages()
    {
        var repository = new ArticleRepository(new Mock<ILogger<ArticleRepository>>().Object, _redisFixture.Connection);
        var result = await repository.GetAsync(new ArticleGetManyQuery
        {
            Category = ArticleCategory.NEWS,
            IsLiveNews = false,
            IsValidForAnalysis = false,
            Languages = ["en", "fi", "sv"],
            Offset = 0,
            PageSize = 10
        });
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
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
            Source = "repository_integration_test_get_by_source",
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1,
            IsValidForAnalysis = true,
        };
    }
    [Fact]
    public async Task GetBySourceAsync_OK()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
        var publishHandler = new ArticleAddCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticleForOk();
        var publishEvent = await publishHandler.Handle(new ArticleAddCommand { Article = article }, CancellationToken.None);
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
            Source = "integration_test_get_by_isvalidated",
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1,
            IsValidForAnalysis = false,
        };
    }
    [Fact]
    public async Task UpdateStuff()
    {
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForIsValidated.ToString()));
        var publishHandler = new ArticleAddCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
        var article = TestArticleForIsValidated();
        var publishEvent = await publishHandler.Handle(new ArticleAddCommand { Article = article }, CancellationToken.None);
        Assert.NotNull(publishEvent);
        Assert.Equal(_idForIsValidated, publishEvent.Id);

        var repository = new ArticleRepository(
            new Mock<ILogger<ArticleRepository>>().Object,
             _redisFixture.Connection);

        // act (test run paraller so there can be many invalid articles)
        var result = await repository.GetAsync(new ArticleGetManyQuery
        {
            Category = ArticleCategory.BOGUS,
            IsValidForAnalysis = false,
            Offset = 0,
            PageSize = 100,
        });
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        var articleCopy = result.Items.Single(i => i.Id == _idForIsValidated);
        Assert.True(article == articleCopy);

        // polarity update
        await repository.UpdatePolarityAsync(_idForIsValidated, 2, Polarity.Negative);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(2, article.PolarityVersion);
        Assert.Equal(Polarity.Negative, article.Polarity);

        // token count update
        await repository.UpdateTokenCountAsync(_idForIsValidated, 100);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(100, article.TokenCount);

        // summary update
        await repository.UpdateSummaryAsync(_idForIsValidated, 1, "summary");
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.SummaryVersion);
        Assert.Equal("summary", article.Analysis.Summary);

        // positive things
        await repository.UpdatePositiveThingsAsync(_idForIsValidated, 1, "positive");
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.PositiveThingsVersion);
        Assert.Equal("positive", article.Analysis.PositiveThings);

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
