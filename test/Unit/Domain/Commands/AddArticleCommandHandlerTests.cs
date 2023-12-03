using AJE.Domain.Commands;
using AJE.Domain.Data;
using AJE.Domain.Entities;
using AJE.Domain.Enums;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Commands;

public class AddArticleCommandHandlerTests
{
    private static Article TestArticle =>
        new()
        {
            Id = Guid.Parse("12300000-1200-1200-1200-000000000034"),
            Category = ArticleCategory.BOGUS,
            Title = "AJE is born",
            Modified = new DateTime(1980, 9, 12, 12, 00, 12).Ticks,
            Published = true,
            Source = "https://www.anttieskola.com",
            Language = "en",
            Content = new EquatableList<MarkdownElement>
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
        var mockArticleRepository = new Mock<IArticleRepository>();
        var mockArticleEventHandler = new Mock<IArticleEventHandler>();
        var handler = new AddArticleCommandHandler(mockArticleRepository.Object, mockArticleEventHandler.Object);

        // act
        var article = TestArticle;
        var command = new AddArticleCommand { Article = article };
        var result = handler.Handle(command, CancellationToken.None).Result;

        // assert
        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
    }
}
