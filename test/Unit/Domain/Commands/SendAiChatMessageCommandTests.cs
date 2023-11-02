using AJE.Domain.Ai;
using AJE.Domain.Commands;
using AJE.Domain.Data;
using AJE.Domain.Events;

namespace AJE.Test.UnitDomain.Commands;

public class SendAiChatMessageCommandTests
{
    // TODO (continue)
    [Fact]
    public async Task Ok()
    {
        var mockAiChatReposiroty = new Mock<IAiChatRepository>();
        var mockAiModel = new Mock<IAiModel>();
        var handler = new SendAiChatMessageCommandHandler(mockAiChatReposiroty.Object, mockAiModel.Object);
        var result = await handler.Handle(new SendAiChatMessageCommand
        {
            ChatId = Guid.NewGuid(),
            Message = "Hello",
            Output = new MemoryStream()
        }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result is AiChatStartedEvent);
    }
}
