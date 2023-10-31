using AJE.Domain.Entities;
using AJE.Domain.Enums;

namespace AJE.Test.Integration;

public static class TestArticle
{
    public static Article Article_01(Guid id, string source)
    {

        return new Article
        {
            Id = id,
            Category = Category.BOGUS,
            Title = "test",
            Modified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Published = true,
            Source = source,
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
                new MarkdownHeaderElement
                {
                    Level = 2,
                    Text = "This is header 2"
                },
                new MarkdownTextElement
                {
                    Text = "This is another paragraph"
                },
            }
        };
    }
}
