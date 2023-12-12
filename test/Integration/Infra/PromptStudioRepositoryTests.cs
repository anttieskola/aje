using AJE.Domain.Entities;
using AJE.Domain.Queries;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

/// <summary>
/// Tests require redis running
/// </summary>
public class PromptStudioRepositoryTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly PromptStudioIndex _index = new();
    public PromptStudioRepositoryTests(RedisFixture fixture)
    {
        _redisFixture = fixture;
    }

    private readonly Guid _idForOk = new("00000000-0000-0000-0000-000000000100");

    [Fact]
    public async Task LifeCycle()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
        var repository = new PromptStudioRepository(
            new Mock<ILogger<PromptStudioRepository>>().Object,
             _redisFixture.Connection);
        var options = new PromptStudioOptions { SessionId = _idForOk };

        // act: add
        var session = await repository.AddAsync(options);
        Assert.NotNull(session);
        Assert.Empty(session.Runs);

        // act add run
        var run = new PromptStudioRun
        {
            RunId = Guid.NewGuid(),
            EntityName = "test",
            SystemInstructions = ["yes", "no", "maybe"],
            Context = "test context",
            Result = "test result",
            Model = "test model",
            NumberOfTokensEvaluated = 1,
            NumberOfTokensContext = 2,
        };
        session = await repository.AddRunAsync(_idForOk, run);
        Assert.NotNull(session);
        Assert.Single(session.Runs);
        Assert.Equal(run, session.Runs[0]);

        // act: get headers
        var headers = await repository.GetHeadersAsync(new PromptStudioGetManySessionHeadersQuery { Offset = 0, PageSize = 1 });
        Assert.NotNull(headers);
        Assert.NotEmpty(headers.Items);

        // act: update title
        await repository.SaveTitleAsync(_idForOk, "test title");
        session = await repository.GetAsync(_idForOk);
        var currentTicks = session.Modified;
        Assert.NotNull(session);
        Assert.Equal("test title", session.Title);

        // act: update temperature
        await repository.SaveTemperatureAsync(_idForOk, 0.5);
        session = await repository.GetAsync(_idForOk);
        Assert.NotNull(session);
        Assert.True(session.Modified > currentTicks);
        currentTicks = session.Modified;
        Assert.Equal(0.5, session.Temperature);

        // act: update number of tokens evaluated
        await repository.SaveNumberOfTokensToPredictAsync(_idForOk, 1024);
        session = await repository.GetAsync(_idForOk);
        Assert.NotNull(session);
        Assert.True(session.Modified > currentTicks);
        currentTicks = session.Modified;
        Assert.Equal(1024, session.NumberOfTokensToPredict);

        // act: update entity name
        await repository.SaveEntityNameAsync(_idForOk, "test entity name");
        session = await repository.GetAsync(_idForOk);
        Assert.NotNull(session);
        Assert.True(session.Modified > currentTicks);
        currentTicks = session.Modified;
        Assert.Equal("test entity name", session.EntityName);

        // act: update system instructions
        await repository.SaveSystemInstructionsAsync(_idForOk, ["You are an assistant that solves alll problems", "You start with bringing peace to the world"]);
        session = await repository.GetAsync(_idForOk);
        Assert.NotNull(session);
        Assert.True(session.Modified > currentTicks);
        currentTicks = session.Modified;
        Assert.Equal(2, session.SystemInstructions.Count);
        Assert.Equal("You are an assistant that solves alll problems", session.SystemInstructions[0]);
        Assert.Equal("You start with bringing peace to the world", session.SystemInstructions[1]);

        // act: update context
        await repository.SaveContextAsync(_idForOk, "test context");
        session = await repository.GetAsync(_idForOk);
        Assert.NotNull(session);
        Assert.True(session.Modified > currentTicks);
        Assert.Equal("test context", session.Context);

        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
    }
}
