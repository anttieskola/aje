using AJE.Application;

namespace AJE.UnitTests;

public class RedisTokenizerTest
{
    [Theory]
    [InlineData("", "")]
    [InlineData("https://rosetta.net/decentralized/human/refined-granite-shoes", "https\\:\\/\\/rosetta\\.net\\/decentralized\\/human\\/refined\\-granite\\-shoes")]
    public void Test(string input, string expected)
    {
        var escaped = input.RedisEscape();
        Assert.Equal(expected, escaped);
    }
}
