using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AJE.Domain.Ai;
using AJE.Domain.Commands;
using AJE.Domain.Events;
using AJE.Domain.Queries;
using AJE.Infra.Ai;
using AJE.Infra.Redis.Data;
using AJE.Infra.Redis.Events;
using AJE.Infra.Redis.Indexes;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Domain;

/// <summary>
/// Tests require redis running on localhost:6379
/// Tests require llama.cpp running on localhost:8080
/// </summary>
// maybe this warning is bug as it only shows where where two fixtures are used?
#pragma warning disable xUnit1033
public class AiChatTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>
{
#pragma warning restore xUnit1033
    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;
    private readonly AiChatIndex _index = new();

    public AiChatTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
    }

    private readonly Guid _idChat = new("00000000-0000-0000-1000-000000000001");

    [Fact]
    public async Task LifeCycle()
    {
        // arrange
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idChat.ToString()));

        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/tmp" };
        var aiChatRepository = new AiChatRepository(new Mock<ILogger<AiChatRepository>>().Object, _redisFixture.Connection);
        var aiEventHandler = new AiChatEventHandler(_redisFixture.Connection);
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _httpClientFixture.HttpClientFactory);
        var antai = new AntaiChatML();
        var startHandler = new AiChatStartCommandHandler(aiChatRepository, aiEventHandler);
        var sendHandler = new AiChatSendMessageCommandHandler(aiChatRepository, aiEventHandler, antai, aiModel);
        var getHandler = new AiChatGetQueryHandler(aiChatRepository);

        var tokens = new StringBuilder();
        var startEvents = new List<AiChatStartedEvent>();
        var interactionEvents = new List<AiChatInteractionEvent>();
        _redisFixture.Connection.GetSubscriber().Subscribe(_index.Channel, OnMessage);
        void OnMessage(RedisChannel channel, RedisValue message)
        {
            if (message.HasValue)
            {
                var msg = JsonSerializer.Deserialize<AiChatEvent>(message.ToString());
                if (msg != null && msg.IsTest && msg.ChatId == _idChat && startEvents != null && interactionEvents != null && tokens != null)
                {
                    if (msg is AiChatStartedEvent startEvent)
                        startEvents.Add(startEvent);
                    else if (msg is AiChatInteractionEvent interactionEvent)
                        interactionEvents.Add(interactionEvent);
                    else if (msg is AiChatTokenEvent tokenEvent)
                        tokens.Append(tokenEvent.Token);
                }
            }
        }

        // act: start chat
        var aiEvent = await startHandler.Handle(new AiChatStartCommand { IsTest = true, Id = _idChat }, CancellationToken.None);
        var startEvent = aiEvent as AiChatStartedEvent;
        Assert.NotNull(startEvent);
        Assert.Equal(_idChat, startEvent.ChatId);

        // act: say hello to AI
        aiEvent = await sendHandler.Handle(new AiChatSendMessageCommand { IsTest = true, ChatId = _idChat, Message = "Hello my name is IntegrationTest, how are you?" }, CancellationToken.None);
        var messageEvent = aiEvent as AiChatInteractionEvent;
        Assert.NotNull(messageEvent);
        Assert.Equal(_idChat, messageEvent.ChatId);
        Assert.NotEmpty(tokens.ToString().Trim());
        Assert.Equal(messageEvent.Output, tokens.ToString().Trim());

        // act: ask AI what was my name again to test context contains history of past interactions
        tokens.Clear();
        aiEvent = await sendHandler.Handle(new AiChatSendMessageCommand { IsTest = true, ChatId = _idChat, Message = "What was my name again?" }, CancellationToken.None);
        await Task.Delay(TimeSpan.FromSeconds(1));
        messageEvent = aiEvent as AiChatInteractionEvent;
        Assert.NotNull(messageEvent);
        Assert.Equal(_idChat, messageEvent.ChatId);
        Assert.NotEmpty(tokens.ToString());
        Assert.Equal(messageEvent.Output.Trim(), tokens.ToString().Trim());
        Assert.Contains("IntegrationTest", messageEvent.Output);
        Assert.Contains("IntegrationTest", tokens.ToString());

        // Get chat
        var chat = await getHandler.Handle(new AiChatGetQuery { Id = _idChat }, CancellationToken.None);
        Assert.NotNull(chat);
        Assert.Equal(_idChat, chat.ChatId);
        Assert.Equal(2, chat.Interactions.Count);

        // cleanup
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.Single(startEvents);
        Assert.Equal(2, interactionEvents.Count);

        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idChat.ToString()));
    }
}
