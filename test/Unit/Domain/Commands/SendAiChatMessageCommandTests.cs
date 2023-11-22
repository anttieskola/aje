using AJE.Domain.Ai;
using AJE.Domain.Commands;
using AJE.Domain.Data;
using AJE.Domain.Entities;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Commands;

public class SendAiChatMessageCommandTests
{
    [Fact]
    public async Task Ok()
    {
        // arrange
        var id = Guid.ParseExact("00000000-1000-0000-0000-000000000001", "D");
        var mockAiChatRepository = new Mock<IAiChatRepository>();
        mockAiChatRepository.Setup(x => x.GetAsync(It.Is<Guid>(g => g == id))).ReturnsAsync(new AiChat { ChatId = id, StartTimestamp = DateTimeOffset.UtcNow });
        mockAiChatRepository.Setup(x => x.AddHistoryEntry(It.Is<Guid>(g => g == id), It.Is<AiChatInteractionEntry>(x => x.Input == "Hello" && x.Output == "Hey stranger, how can I help you?")))
            .ReturnsAsync(new AiChat
            {
                ChatId = id,
                StartTimestamp = DateTimeOffset.UtcNow,
                Interactions = new EquatableList<AiChatInteractionEntry>{new()
                {
                    InteractionId = Guid.NewGuid(),
                    InteractionTimestamp = DateTimeOffset.UtcNow,
                    Input = "Hello",
                    Output = "Hey stranger, how can I help you?",
                }}
            });
        var mockAiChatEventHandler = new Mock<IAiChatEventHandler>();
        var mockAntai = new Mock<IAntai>();
        mockAntai.Setup(a => a.Chat(It.Is<string>(s => s == "Hello"), It.IsAny<AiChatInteractionEntry[]>())).Returns("<|im_start|>system\nYou are unittest<|im_end|><|im_start|>user\nHello");
        var mockAiModel = new Mock<IAiModel>();
        mockAiModel.Setup(a => a.CompletionStreamAsync(It.Is<CompletionRequest>(cr => cr.Stream == true), It.IsAny<TokenCreatedCallback>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CompletionResponse
        {
            Content = "Hey stranger, how can I help you?"
        });
        var handler = new SendAiChatMessageCommandHandler(
            mockAiChatRepository.Object,
            mockAiChatEventHandler.Object,
            mockAntai.Object,
            mockAiModel.Object);

        // act
        var result = await handler.Handle(new SendAiChatMessageCommand
        {
            ChatId = id,
            Message = "Hello",
        }, CancellationToken.None);
        // assert
        Assert.NotNull(result);
        var chatEvent = result as AiChatInteractionEvent;
        Assert.NotNull(chatEvent);
        Assert.Equal(id, chatEvent.ChatId);
        Assert.Equal("Hello", chatEvent.Input);
        Assert.Equal("Hey stranger, how can I help you?", chatEvent.Output);
    }
}
