namespace UnitTests;

public class Serialization
{
    public static Article GenerateFakeArticle()
    {
        var faker = new Faker("en");
        var id = Guid.NewGuid();        
        var a = new Article()
        {
            Id = id,
            Published = faker.Random.Bool(),
            Title = faker.Commerce.ProductName(),
            Content = faker.Commerce.ProductDescription(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Chat = new List<ChatMessage>
                    {
                        new ChatMessage
                        {
                            User = faker.Internet.UserName(),
                            Message = faker.Hacker.Phrase(),
                        },
                        new ChatMessage
                        {
                            User = faker.Internet.UserName(),
                            Message = faker.Hacker.Phrase(),
                        },
                        new ChatMessage
                        {
                            User = faker.Internet.UserName(),
                            Message = faker.Hacker.Phrase(),
                        },
                        new ChatMessage
                        {
                            User = faker.Internet.UserName(),
                            Message = faker.Hacker.Phrase(),
                        },
                        new ChatMessage
                        {
                            User = faker.Internet.UserName(),
                            Message = faker.Hacker.Phrase(),
                        },
                    },
        };
        return a;
    }

    [Fact]
    public void ArticleIsSerializationSuccessfull()
    {
        var article = GenerateFakeArticle(); 
        
    }
}