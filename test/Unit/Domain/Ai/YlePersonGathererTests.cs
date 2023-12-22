using AJE.Domain.Ai;
using AJE.Domain.Entities;

namespace AJE.Test.Unit;

public class YlePersonGathererTests
{
    [Fact]
    public async Task Ok()
    {
        var pg = new YlePersonGatherer();
        var persons = await pg.GetPersonsAsync(
        [
            new MarkdownTextElement
            {
                Text = "Yle meteorologist **Matti Huutonen** says that aurora activity tends to be stronger in the autumn months of September-October or else in early spring, especially around the time of the equinox. **Antti Eskola** Disagrees.",
            },
        ], CancellationToken.None);
        Assert.Equal(2, persons.Count);
        Assert.Equal("Matti Huutonen", persons[0].Name);
        Assert.Equal("Antti Eskola", persons[1].Name);
    }
}
