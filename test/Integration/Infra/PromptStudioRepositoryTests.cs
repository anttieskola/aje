using AJE.Domain.Entities;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

/// <summary>
/// Tests require redis running on localhost:6379
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
            SystemInstructions = new EquatableList<string> { "yes", "no", "maybe" },
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
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idForOk.ToString()));
    }
}
