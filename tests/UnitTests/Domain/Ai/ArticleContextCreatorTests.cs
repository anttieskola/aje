using AJE.Domain.Ai;
using AJE.Domain.Entities;

namespace AJE.UnitTests.Domain.Ai;

public class ArticleContextCreatorTests
{
    [Fact]
    public void Test()
    {
        var mockSimplifier = new Mock<ISimplifier>();
        mockSimplifier.Setup(x => x.Simplify(It.IsAny<string>())).Returns((string s) => s);
        var article = new Article
        {
            Content = new List<MarkdownElement>
            {
                new MarkdownHeaderElement{
                    Level = 1,
                    Text = "Do we want headers?"
                },
                new MarkdownTextElement{
                    Text = "Text should always be included"
                }
            },
        };
        var creator = new ArticleContextCreator(mockSimplifier.Object);
        var context = creator.Create(article);
        Assert.Equal("Text should always be included", context);
    }
}
