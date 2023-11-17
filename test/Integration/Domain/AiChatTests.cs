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
    private readonly IRedisIndex _index = new AiChatIndex();

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
        var configuration = new LlamaConfiguration { Host = "http://localhost:8080", LogFolder = "/tmp" };

        var aiChatRepository = new AiChatRepository(new Mock<ILogger<AiChatRepository>>().Object, _redisFixture.Connection);
        var aiEventHandler = new AiChatEventHandler(_redisFixture.Connection);
        var aiModel = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _httpClientFixture.HttpClientFactory);
        var antai = new AntaiChatML();
        var startHandler = new StartAiChatCommandHandler(aiChatRepository, aiEventHandler);
        var sendHandler = new SendAiChatMessageCommandHandler(aiChatRepository, aiEventHandler, antai, aiModel);
        var getHandler = new GetAiChatQueryHandler(aiChatRepository);

        // act: start chat
        var aiEvent = await startHandler.Handle(new StartAiChatCommand { Id = _idChat }, CancellationToken.None);
        var startEvent = aiEvent as AiChatStartedEvent;
        Assert.NotNull(startEvent);
        Assert.Equal(_idChat, startEvent.ChatId);

        using var outputOne = new MemoryStream();
        // act: say hello to AI
        aiEvent = await sendHandler.Handle(new SendAiChatMessageCommand { Id = _idChat, Message = "Hello my name is IntegrationTest, how are you?", Output = outputOne }, CancellationToken.None);
        var messageEvent = aiEvent as AiChatInteractionEvent;
        Assert.NotNull(messageEvent);
        Assert.Equal(_idChat, messageEvent.ChatId);
        outputOne.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(outputOne))
        {
            var output = await reader.ReadToEndAsync();
            Assert.NotEmpty(output);
            Assert.Equal(messageEvent.Output, output.Trim());
        }

        // act: ask AI what was my name again to test history context
        using var outputTwo = new MemoryStream();
        aiEvent = await sendHandler.Handle(new SendAiChatMessageCommand { Id = _idChat, Message = "What was my name again?", Output = outputTwo }, CancellationToken.None);
        messageEvent = aiEvent as AiChatInteractionEvent;
        Assert.NotNull(messageEvent);
        Assert.Equal(_idChat, messageEvent.ChatId);
        outputTwo.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(outputTwo))
        {
            var output = await reader.ReadToEndAsync();
            Assert.NotEmpty(output);
            Assert.Equal(messageEvent.Output, output.Trim());
            Assert.Contains("IntegrationTest", output);
            Assert.Contains("IntegrationTest", messageEvent.Output);
        }

        // Get chat
        var chat = await getHandler.Handle(new GetAiChatQuery { Id = _idChat }, CancellationToken.None);
        Assert.NotNull(chat);
        Assert.Equal(_idChat, chat.ChatId);
        Assert.Equal(2, chat.Interactions.Count);

        // cleanup
        await _redisFixture.Database.KeyDeleteAsync(_index.RedisId(_idChat.ToString()));
    }
}
