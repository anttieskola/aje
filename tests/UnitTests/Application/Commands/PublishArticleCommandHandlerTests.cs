using AJE.Application.Commands;
using AJE.Domain.Commands;

namespace AJE.UnitTests.Application.Commands;

public class PublishArticleCommandHandlerTests
{
    // TODO
    [Fact]
    public void Ok()
    {
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();

        mockConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);

        var handler = new PublishArticleCommandHandler(mockConnectionMultiplexer.Object);
        var id = Guid.NewGuid();
        var command = new PublishArticleCommand
        {
            Article = new Article
            {
                Id = id,
                Title = "test",
            }
        };

        var result = handler.Handle(command, CancellationToken.None).Result;
        Assert.NotNull(result);
        Assert.Equal(command.Article.Id, result.Id);
    }
}
