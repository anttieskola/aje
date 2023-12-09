using AJE.Domain.Commands;
using AJE.Domain.Data;
using AJE.Domain.Entities;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Commands;

public class AiChatStartCommandTests
{
    [Fact]
    public async Task Ok()
    {
        var id = Guid.ParseExact("00000000-0000-0000-0000-000000000001", "D");
        var mockAiChatRepository = new Mock<IAiChatRepository>();
        mockAiChatRepository.Setup(x => x.AddAsync(It.IsAny<AiChatOptions>())).ReturnsAsync(new AiChat { ChatId = id, StartTimestamp = DateTimeOffset.UtcNow });
        var mockAiChatEventHandler = new Mock<IAiChatEventHandler>();
        var handler = new AiChatStartCommandHandler(mockAiChatRepository.Object, mockAiChatEventHandler.Object);
        var result = await handler.Handle(new AiChatStartCommand { Id = id }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result is AiChatStartedEvent);
        Assert.NotEqual(Guid.Empty, result.ChatId);
        Assert.Equal(id, result.ChatId);
        mockAiChatEventHandler.Verify(e => e.SendAsync(It.IsAny<AiChatStartedEvent>()), Times.Once);
    }
}
