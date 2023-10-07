
using NRedisStack;
using NRedisStack.RedisStackCommands;

namespace AJE.UnitTests.Application.Queries;

public class ArticleExistsQueryTests
{
    [Fact]
    public void Test()
    {
        // Arrange
        var mockConnection = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();

        mockConnection
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDatabase.Object);

        var mockSearchCommands = new Mock<ISearchCommands>();

        // Can't mock the implementations
        // mockDatabase
        //     .Setup(d => d.FT(It.IsAny<int>()))
        //     .Returns(shit);

    }
}
