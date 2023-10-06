using System.Text.Json;

namespace AJE.UnitTests;

public class SerializationTests
{
    [Fact]
    public void ArticleWithContentTypes()
    {
        var original = new Article
        {
            Id = Guid.Parse("12300000-1200-1200-1200-000000000034"),
            Category = ArticleCategory.NEWS,
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
                new MarkdownHeaderElement{
                    Level = 2,
                    Text = "This is header 2"
                },
                new MarkdownTextElement{
                    Text = "This is another paragraph"
                },
            },
        };

        var json = JsonSerializer.Serialize(original);
        var copy = JsonSerializer.Deserialize<Article>(json);

        Assert.NotNull(copy);
        Assert.Equal(original.Id, copy.Id);
        Assert.Equal(original.Category, copy.Category);
        Assert.Equal(original.Title, copy.Title);
        Assert.Equal(original.Modified, copy.Modified);
        Assert.Equal(original.Published, copy.Published);
        Assert.Equal(original.Source, copy.Source);
        Assert.Equal(original.Language, copy.Language);
        Assert.Equal(original.Content.Count(), copy.Content.Count());

        var h1 = copy.Content.ElementAt(0) as MarkdownHeaderElement;
        Assert.NotNull(h1);
        Assert.Equal(1, h1.Level);
        Assert.Equal("This is header 1", h1.Text);

        var p1 = copy.Content.ElementAt(1) as MarkdownTextElement;
        Assert.NotNull(p1);
        Assert.Equal("This is a paragraph", p1.Text);

        var h2 = copy.Content.ElementAt(2) as MarkdownHeaderElement;
        Assert.NotNull(h2);
        Assert.Equal(2, h2.Level);
        Assert.Equal("This is header 2", h2.Text);

        var p2 = copy.Content.ElementAt(3) as MarkdownTextElement;
        Assert.NotNull(p2);
        Assert.Equal("This is another paragraph", p2.Text);
    }
}
