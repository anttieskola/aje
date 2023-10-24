using AJE.Application.Commands;
using AJE.Domain.Commands;
using AJE.Domain.Entities;
using AJE.Domain.Enums;
using AJE.Domain.Exceptions;

namespace AJE.UnitTests.Application.Commands;

public class PublishArticleCommandHandlerTests
{
    private Article TestArticle =>
        new Article
        {
            Id = Guid.Parse("12300000-1200-1200-1200-000000000034"),
            Category = Category.NEWS,
            Title = "AJE is born",
            Modified = new DateTime(1980, 9, 12, 12, 00, 12).Ticks,
            Published = true,
            Source = "https://www.anttieskola.com",
            Language = "en",
            Content = new List<MarkdownElement>
            {
                new MarkdownHeaderElement{
                    Level = 1,
                    Text = "This is header 1"
                },
                new MarkdownTextElement{
                    Text = "This is a paragraph"
                },
            },
        };


    [Fact]
    public void Ok()
    {
        // arrange
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();
        mockConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
        mockDatabase.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
        mockDatabase.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object[]>())).ReturnsAsync(RedisResult.Create("OK", ResultType.SimpleString));
        var handler = new PublishArticleCommandHandler(mockConnectionMultiplexer.Object);

        // act
        var article = TestArticle;
        var command = new PublishArticleCommand { Article = article };
        var result = handler.Handle(command, CancellationToken.None).Result;

        // assert
        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
    }

    [Fact]
    public void Error_KeyExists()
    {
        // arrange
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();
        mockConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
        mockDatabase.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
        var handler = new PublishArticleCommandHandler(mockConnectionMultiplexer.Object);

        // act & assert
        var article = TestArticle;
        var command = new PublishArticleCommand { Article = article };
        Assert.ThrowsAsync<KeyExistsException>(async () => await handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public void Error_DataException()
    {
        // arrange
        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();
        mockConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
        mockDatabase.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
        mockDatabase.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object[]>())).ReturnsAsync(RedisResult.Create("ERROR", ResultType.SimpleString));
        var handler = new PublishArticleCommandHandler(mockConnectionMultiplexer.Object);

        // act & assert
        var article = TestArticle;
        var command = new PublishArticleCommand { Article = article };
        Assert.ThrowsAsync<DataException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}
