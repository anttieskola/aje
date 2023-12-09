using AJE.Domain;
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
public class TrendRepositoryTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly ArticleIndex _index = new();
    public TrendRepositoryTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    #region test articles

    private static Article DaylyArticle_Positive(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "daily test positive article",
            Modified = DateTimeOffset.UtcNow.Date.AddDays(-1).AddHours(12).Ticks, // one day ago, 12:00
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1
        };
    }

    private static Article HourlyArticle_Positive(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "hourly test positive article",
            Modified = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, DateTimeOffset.UtcNow.Day, DateTimeOffset.UtcNow.Hour, 0, 0, TimeSpan.Zero).AddHours(-1).Ticks, // one hour ago
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Positive,
            PolarityVersion = 1
        };
    }

    private static Article DaylyArticle_Neutral(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "daily test neutral article",
            Modified = DateTimeOffset.UtcNow.Date.AddDays(-2).AddHours(12).Ticks, // two day ago, 12:00
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Neutral,
            PolarityVersion = 1
        };
    }

    private static Article HourlyArticle_Neutral(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "hourly test neutral article",
            Modified = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, DateTimeOffset.UtcNow.Day, DateTimeOffset.UtcNow.Hour, 0, 0, TimeSpan.Zero).AddHours(-2).Ticks, // two hour ago
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Neutral,
            PolarityVersion = 1
        };
    }

    private static Article DaylyArticle_Negative(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "daily test negative article",
            Modified = DateTimeOffset.UtcNow.Date.AddDays(-3).AddHours(12).Ticks, // three day ago, 12:00
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Negative,
            PolarityVersion = 1
        };
    }

    private static Article HourlyArticle_Negative(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "hourly test negative article",
            Modified = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, DateTimeOffset.UtcNow.Day, DateTimeOffset.UtcNow.Hour, 0, 0, TimeSpan.Zero).AddHours(-3).Ticks, // three hour ago
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Negative,
            PolarityVersion = 1
        };
    }

    private static Article DaylyArticle_Unknown(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "daily test unknown article",
            Modified = DateTimeOffset.UtcNow.Date.AddDays(-4).AddHours(12).Ticks, // four day ago, 12:00
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Unknown,
            PolarityVersion = 1
        };
    }

    private static Article HourlyArticle_Unknown(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "hourly test unknown article",
            Modified = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, DateTimeOffset.UtcNow.Day, DateTimeOffset.UtcNow.Hour, 0, 0, TimeSpan.Zero).AddHours(-4).Ticks, // four hour ago
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
            Polarity = Polarity.Unknown,
            PolarityVersion = 1
        };
    }

    private static Article DaylyArticle_NoPolarity(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "daily test without polarity",
            Modified = DateTimeOffset.UtcNow.Date.AddDays(-5).AddHours(12).Ticks, // five day ago, 12:00
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
        };
    }

    private static Article HourlyArticle_NoPolarity(Guid id, string source)
    {
        return new Article
        {
            Id = id,
            Category = ArticleCategory.BOGUS,
            Title = "hourly test without polarity",
            Modified = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, DateTimeOffset.UtcNow.Day, DateTimeOffset.UtcNow.Hour, 0, 0, TimeSpan.Zero).AddHours(-5).Ticks, // five hours ago
            Published = true,
            Source = source,
            Language = "en",
            Content = TestArticle.Content,
        };
    }

    #endregion test articles

    /// <summary>
    /// Combined two tests so they don't run same time (messes up calculations)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Calculations()
    {
        var idPositive = Guid.ParseExact("00000000-0000-0000-0000-000000000021", "D");
        var idNeutral = Guid.ParseExact("00000000-0000-0000-0000-000000000022", "D");
        var idNegate = Guid.ParseExact("00000000-0000-0000-0000-000000000023", "D");
        var idUnknown = Guid.ParseExact("00000000-0000-0000-0000-000000000024", "D");
        var idNoPolarity = Guid.ParseExact("00000000-0000-0000-0000-000000000025", "D");

        // Daily part
        {
            // clean
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idPositive.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNeutral.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNegate.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idUnknown.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNoPolarity.ToString()));

            // arrange
            var publishHandler = new ArticleAddCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
            await publishHandler.Handle(new ArticleAddCommand { Article = DaylyArticle_Negative(idNegate, "daily negative") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = DaylyArticle_Neutral(idNeutral, "daily neutral") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = DaylyArticle_Positive(idPositive, "daily positive") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = DaylyArticle_Unknown(idUnknown, "daily unknown") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = DaylyArticle_NoPolarity(idNoPolarity, "daily nopolarity") }, CancellationToken.None);

            var handler = new ArticleGetSentimentPolarityTrendsQueryHandler(new TrendRepository(new Mock<ILogger<TrendRepository>>().Object, _redisFixture.Connection));
            // query last six days + today
            var results = await handler.Handle(new ArticleGetSentimentPolarityTrendsQuery
            {
                ArticleCategory = ArticleCategory.BOGUS,
                TimePeriod = TimePeriod.Day,
                Start = DateTimeOffset.UtcNow.AddDays(-5),
                End = DateTimeOffset.UtcNow
            }, CancellationToken.None);
            Assert.NotNull(results);

            // If UTC time is before midnight and local time after we don't get today's results
            var isTodayIncluded = DateTimeOffset.UtcNow.Date == DateTimeOffset.Now.Date;
            if (isTodayIncluded)
            {
                Assert.Equal(6, results.Length); // -5, -4 -3, -2, -1, 0
                Assert.Empty(results[5].Items); // today
                Assert.Single(results[4].Items); // yesterday
                Assert.Equal(1, results[4].PositiveCount);
                Assert.Single(results[3].Items); // two days ago
                Assert.Equal(1, results[3].NeutralCount);
                Assert.Single(results[2].Items); // three days ago
                Assert.Equal(1, results[2].NegativeCount);
                Assert.Single(results[1].Items); // four days ago
                Assert.Equal(1, results[1].UnknownCount);
                Assert.Single(results[0].Items); // five days ago
                Assert.Equal(1, results[0].UnknownCount);
            }
            else
            {
                // Why when local time is in next day and utc not we get another empty result?
                // example utc date 09, local date 10
                // [6] == empty day 10
                // [5] == empty day 09
                Assert.Equal(7, results.Length);
                Assert.Single(results[4].Items); // yesterday (in utc, but two days ago as local)
                Assert.Equal(1, results[4].PositiveCount);
                Assert.Single(results[3].Items); // two days ago
                Assert.Equal(1, results[3].NeutralCount);
                Assert.Single(results[2].Items); // three days ago
                Assert.Equal(1, results[2].NegativeCount);
                Assert.Single(results[1].Items); // four days ago
                Assert.Equal(1, results[1].UnknownCount);
                Assert.Single(results[0].Items); // five days ago
                Assert.Equal(1, results[0].UnknownCount);
            }

            // clean
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idPositive.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNeutral.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNegate.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idUnknown.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNoPolarity.ToString()));
        }
        // hourly part
        {
            // clean
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idPositive.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNeutral.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNegate.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idUnknown.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNoPolarity.ToString()));

            // arrange
            var publishHandler = new ArticleAddCommandHandler(_redisFixture.ArticleRepository, new Mock<IArticleEventHandler>().Object);
            await publishHandler.Handle(new ArticleAddCommand { Article = HourlyArticle_Negative(idNegate, "hourly negative") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = HourlyArticle_Neutral(idNeutral, "hourly neutral") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = HourlyArticle_Positive(idPositive, "hourly positive") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = HourlyArticle_Unknown(idUnknown, "hourly unknown") }, CancellationToken.None);
            await publishHandler.Handle(new ArticleAddCommand { Article = HourlyArticle_NoPolarity(idNoPolarity, "hourly nopolarity") }, CancellationToken.None);

            var handler = new ArticleGetSentimentPolarityTrendsQueryHandler(new TrendRepository(new Mock<ILogger<TrendRepository>>().Object, _redisFixture.Connection));
            // query last six days + today
            var results = await handler.Handle(new ArticleGetSentimentPolarityTrendsQuery
            {
                ArticleCategory = ArticleCategory.BOGUS,
                TimePeriod = TimePeriod.Hour,
                Start = DateTimeOffset.UtcNow.AddHours(-5),
                End = DateTimeOffset.UtcNow
            }, CancellationToken.None);
            Assert.NotNull(results);
            Assert.Equal(6, results.Length); // -5, -4 -3, -2, -1, 0
            Assert.Empty(results[5].Items); // this hour
            Assert.Single(results[4].Items); // last hour
            Assert.Equal(1, results[4].PositiveCount);
            Assert.Single(results[3].Items); // two hours ago
            Assert.Equal(1, results[3].NeutralCount);
            Assert.Single(results[2].Items); // three hours ago
            Assert.Equal(1, results[2].NegativeCount);
            Assert.Single(results[1].Items); // four hours ago
            Assert.Equal(1, results[1].UnknownCount);
            Assert.Single(results[0].Items); // five hours ago
            Assert.Equal(1, results[0].UnknownCount);

            // clean
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idPositive.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNeutral.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNegate.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idUnknown.ToString()));
            await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(idNoPolarity.ToString()));
        }
    }
}
