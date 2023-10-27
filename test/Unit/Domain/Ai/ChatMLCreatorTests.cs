using AJE.Domain.Ai;

namespace AJE.Test.Unit.Domain.Ai;

public class ChatMLCreatorTests
{
    private class TestChatML : ChatMLCreator
    {
        public const string EntityName = "unittest";
        public readonly static string[] SystemInstructions =
        {
            "You are unittest",
            "Your purpose is to test ChatMLCreator"
        };

        public TestChatML()
            : base(EntityName, SystemInstructions)
        {
        }
    }

    [Fact]
    public void Single()
    {
        var test = new TestChatML();
        var prompt = test.Create("This is a unittest. Will the asserts fail or not?");
        var stopWords = test.StopWords;
        Assert.NotNull(prompt);
        Assert.Equal("<|im_start|>system\nYou are unittest\nYour purpose is to test ChatMLCreator\n<|im_end|><|im_start|>context\nThis is a unittest. Will the asserts fail or not?<|im_end|><|im_start|>unittest\n", prompt);
        Assert.NotEmpty(stopWords);
        Assert.Contains("<|im_start|>", stopWords);
        Assert.Contains("<|im_end|>", stopWords);
    }

    [Fact]
    public void History()
    {
        // TODO
    }
}
