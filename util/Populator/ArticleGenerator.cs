namespace AJE.Util.Populator;

internal class ArticleGenerator
{
    private readonly Faker _faker;
    private readonly Random _random;

    public ArticleGenerator(string language)
    {
        _faker = new Faker(language);
        _random = new Random();
    }

    public Article Generate()
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Category = ArticleCategory.BOGUS,
            Title = _faker.Commerce.ProductName(),
            Modified = DateTime.UtcNow.Ticks,
            Published = _faker.Random.Bool(),
            Source = string.Empty,
            Language = "en",

            Content = GenerateContent(),
            Chat = GenerateChat(),
        };
        return article;
    }

    private EquatableList<MarkdownElement> GenerateContent()
    {
        var content = new EquatableList<MarkdownElement>();
        var contentLength = _random.Next(3, 30);
        for (int i = 0; i < contentLength; i++)
        {
            var type = _random.Next(1, 10);
            if (type > 7)
            {
                content.Add(new MarkdownHeaderElement
                {
                    Level = _random.Next(1, 3),
                    Text = _faker.Commerce.ProductName(),
                });
            }
            else
            {
                content.Add(new MarkdownTextElement
                {
                    Text = _faker.Rant.Review(),
                });
            }
        }
        return content;
    }

    private EquatableList<ChatMessage> GenerateChat()
    {
        var messages = new EquatableList<ChatMessage>();
        var chatLength = _random.Next(1, 10);
        for (int i = 0; i < chatLength; i++)
        {
            messages.Add(new ChatMessage
            {
                UserName = _faker.Internet.UserName(),
                Message = _faker.Hacker.Phrase(),
            });
        }
        return messages;
    }
}
