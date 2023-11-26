using AJE.Domain.Queries;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace AJE.Test.Unit.Domain.Queries;
public class GetGuidQueryTests
{
    [Theory]
    [InlineData('0', true)]
    [InlineData('1', true)]
    [InlineData('2', true)]
    [InlineData('3', true)]
    [InlineData('4', true)]
    [InlineData('5', true)]
    [InlineData('6', true)]
    [InlineData('7', true)]
    [InlineData('8', true)]
    [InlineData('9', true)]
    [InlineData('a', true)]
    [InlineData('A', true)]
    [InlineData('b', true)]
    [InlineData('B', true)]
    [InlineData('c', true)]
    [InlineData('C', true)]
    [InlineData('d', true)]
    [InlineData('D', true)]
    [InlineData('e', true)]
    [InlineData('E', true)]
    [InlineData('f', true)]
    [InlineData('F', true)]
    public void GuidCharacterIsValidTest(char c, bool expected)
    {
        var result = GuidCharacter.IsValid(c);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Fail()
    {
        var handler = new GetGuidQueryHandler();

        // category
        var e = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new GetGuidQuery
        {
            Category = string.Empty,
            UniqueString = "https://yle.fi/a/74-20025030",
        }, CancellationToken.None));
        e.Message.Contains("Category is required");

        e = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new GetGuidQuery
        {
            Category = "1000000",
            UniqueString = "https://yle.fi/a/74-20025030",
        }, CancellationToken.None));
        e.Message.Contains("Category must be 8 characters long");

        e = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new GetGuidQuery
        {
            Category = "1000000X",
            UniqueString = string.Empty,
        }, CancellationToken.None));
        e.Message.Contains("Category must be 8 characters long");

        // unique string
        e = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new GetGuidQuery
        {
            Category = "10000000",
            UniqueString = string.Empty,
        }, CancellationToken.None));
        e.Message.Contains("UniqueString is required");

        e = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new GetGuidQuery
        {
            Category = "10000000",
            UniqueString = "abcdef1",
        }, CancellationToken.None));
        e.Message.Contains("UniqueString must contain atleast 8 valid characters");
    }

    [Fact]
    public async Task Ok()
    {
        var handler = new GetGuidQueryHandler();
        var result = await handler.Handle(new GetGuidQuery
        {
            Category = "10000000",
            UniqueString = "https://yle.fi/a/74-20025030",
        }, CancellationToken.None);
        Assert.Equal(Guid.ParseExact("10000000-efa7-4200-2503-000000000000", "D"), result);

        result = await handler.Handle(new GetGuidQuery
        {
            Category = "10000000",
            UniqueString = "https://yle.fi/a/3-8960573",
        }, CancellationToken.None);
        Assert.Equal(Guid.ParseExact("10000000-efa3-8960-5730-000000000000", "D"), result);

        result = await handler.Handle(new GetGuidQuery
        {
            Category = "20000000",
            UniqueString = "https://news.anttieskola.com/123456789012",
        }, CancellationToken.None);
        Assert.Equal(Guid.ParseExact("20000000-eaea-c123-4567-890120000000", "D"), result);
    }
}
