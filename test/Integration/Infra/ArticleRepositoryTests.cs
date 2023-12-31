﻿using AJE.Domain.Commands;
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

    [Fact(Skip = "Does not work with empty redis")]
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

        // summary update
        await repository.UpdateSummaryAsync(_idForIsValidated, 1, "summary");
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.SummaryVersion);
        Assert.Equal("summary", article.Analysis.Summary);

        // positive things
        await repository.UpdatePositiveThingsAsync(_idForIsValidated, 2, [new() { Title = "title", Description = "description" }]);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(2, article.Analysis.PositiveThingsVersion);
        Assert.Single(article.Analysis.PositiveThings);
        Assert.Equal("title", article.Analysis.PositiveThings[0].Title);
        Assert.Equal("description", article.Analysis.PositiveThings[0].Description);

        // locations
        await repository.UpdateLocationsAsync(_idForIsValidated, 1, [new() { Name = "location" }]);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.LocationsVersion);
        Assert.Equal("location", article.Analysis.Locations[0].Name);

        // corporations
        await repository.UpdateCorporationsAsync(_idForIsValidated, 1, [new() { Name = "corporation" }]);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.CorporationsVersion);
        Assert.Equal("corporation", article.Analysis.Corporations[0].Name);

        // organizations
        await repository.UpdateOrganizationsAsync(_idForIsValidated, 1, [new() { Name = "organization" }]);
        article = await repository.GetAsync(_idForIsValidated);
        Assert.NotNull(article);
        Assert.Equal(1, article.Analysis.OrganizationsVersion);
        Assert.Equal("organization", article.Analysis.Organizations[0].Name);

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
