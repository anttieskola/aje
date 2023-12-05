using AJE.Domain.Entities;

namespace AJE.Test.Integration;

public static class TestArticle
{
    public static EquatableList<MarkdownElement> Content
    {
        get
        {
            return new EquatableList<MarkdownElement>
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
            };
        }
    }
}
