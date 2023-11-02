using AJE.Domain.Commands;
using AJE.Domain.Data;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Commands;

public class StartAiChatCommandTests
{
    [Fact]
    public async Task Ok()
    {
        var mockAiChatReposiroty = new Mock<IAiChatRepository>();
        var mockAiChatEventHandler = new Mock<IAiChatEventHandler>();
        var handler = new StartAiChatCommandHandler(mockAiChatReposiroty.Object, mockAiChatEventHandler.Object);
        var result = await handler.Handle(new StartAiChatCommand(), CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result is AiChatStartedEvent);
    }
}
