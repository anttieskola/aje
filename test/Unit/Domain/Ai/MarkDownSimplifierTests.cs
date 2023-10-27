using AJE.Domain.Ai;

namespace AJE.Test.Unit.Domain.Ai;

public class MarkDownSimplifierTests
{
    [Theory]
    [InlineData("hello world", "hello world")]
    [InlineData("вырабатываемой", "вырабатываемой")]
    [InlineData("fitnodahkii livččii", "fitnodahkii livččii")]
    [InlineData("\n\r\t*", "")]
    [InlineData("abudengoin [opiskelija-apurahojen](https://www.karjalansivistysseura.fi/yhdistys/apurahat-ja-kohdeavustukset/) pakičenduaigu", "abudengoin opiskelija-apurahojen pakičenduaigu")]
    [InlineData("This is a sentence, that contains punctuation.", "This is a sentence, that contains punctuation.")]
    [InlineData("This  contains  extra    spaces   ", "This contains extra spaces")]
    public void Simplify(string markdown, string expected)
    {
        var ms = new MarkDownSimplifier();
        var actual = ms.Simplify(markdown);
        Assert.Equal(expected, actual);
    }
}
