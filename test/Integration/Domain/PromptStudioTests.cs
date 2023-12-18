using System.Text;
using System.Text.Json;
using AJE.Domain.Commands;
using AJE.Domain.Events;
using AJE.Infra.Ai;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Events;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

/// <summary>
/// This test requires a running Llama server and Redis server.
/// </summary>
#pragma warning disable xUnit1033
[Collection("Llama")]
public class PromptStudioTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;
    private readonly LlamaQueueFixture _llamaQueueFixture;
    private readonly PromptStudioIndex _index = new();

    public PromptStudioTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture,
        LlamaQueueFixture llamaQueueFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
        _llamaQueueFixture = llamaQueueFixture;
    }

    private readonly Guid _idPromptStudioSession = new("00000000-0000-1000-0000-000000000001");
    private readonly Guid _idPromptStudioRun = new("00000000-0000-1000-0000-000000000002");

    private IServiceProvider CreateMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LlamaApi>))).Returns(new Mock<ILogger<LlamaApi>>().Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IHttpClientFactory))).Returns(_httpClientFixture.HttpClientFactory);
        return mockServiceProvider.Object;
    }

    [Fact]
    public async Task LifeCycle()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idPromptStudioSession.ToString()));
        var promptStudioRepository = new PromptStudioRepository(new Mock<ILogger<PromptStudioRepository>>().Object, _redisFixture.Connection);
        var promptStudioEventHandler = new PromptStudioEventHandler(_redisFixture.Connection);
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);

        var startHandler = new PromptStudioStartCommandHandler(promptStudioRepository, promptStudioEventHandler);
        var runHandler = new PromptStudioRunCommandHandler(promptStudioRepository, promptStudioEventHandler, aiModel);
        var tokens = new StringBuilder();
        var startEvents = new List<PromptStudioStartEvent>();
        var runEvents = new List<PromptStudioRunCompletedEvent>();
        _redisFixture.Connection.GetSubscriber().Subscribe(_index.Channel, OnMessage);
        void OnMessage(RedisChannel channel, RedisValue message)
        {
            if (message.HasValue)
            {
                var msg = JsonSerializer.Deserialize<PromptStudioEvent>(message.ToString());
                if (msg != null && msg.IsTest && msg.SessionId == _idPromptStudioSession && startEvents != null && runEvents != null && tokens != null)
                {
                    if (msg is PromptStudioStartEvent startEvent)
                        startEvents.Add(startEvent);
                    else if (msg is PromptStudioRunCompletedEvent runCompletedEvent)
                        runEvents.Add(runCompletedEvent);
                    else if (msg is PromptStudioRunTokenEvent tokenEvent)
                        tokens.Append(tokenEvent.Token);
                }
            }
        }

        // act start session
        var startEvent = await startHandler.Handle(new PromptStudioStartCommand
        {
            IsTest = true,
            SessionId = _idPromptStudioSession,
        }, CancellationToken.None);
        Assert.NotNull(startEvent);
        Assert.Equal(_idPromptStudioSession, startEvent.SessionId);

        // do a run
        var runEvent = await runHandler.Handle(new PromptStudioRunCommand
        {
            IsTest = true,
            SessionId = _idPromptStudioSession,
            RunId = _idPromptStudioRun,
            EntityName = "assistant",
            SystemInstructions = ["You are an assistant that will examine context and make up a joke about it"],
            Context = "Antti Eskola is a software developer",
            Temperature = 1.2,
            NumberOfTokensToPredict = 256,
        }, CancellationToken.None);
        Assert.NotNull(runEvent);
        Assert.Equal(_idPromptStudioSession, runEvent.SessionId);
        Assert.Equal(_idPromptStudioRun, runEvent.RunId);
        Assert.Equal("Antti Eskola is a software developer", runEvent.Input);
        Assert.NotEmpty(tokens.ToString());
        Assert.NotEmpty(runEvent.Output);
        Assert.NotEmpty(runEvent.Model);
        Assert.True(runEvent.NumberOfTokensEvaluated > 0);
        Assert.True(runEvent.NumberOfTokensContext > 0);

        // cleanup
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idPromptStudioSession.ToString()));
    }
}
