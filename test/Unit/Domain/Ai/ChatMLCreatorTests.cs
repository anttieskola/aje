using AJE.Domain.Ai;
using AJE.Domain.Entities;

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
    public void Context()
    {
        var test = new TestChatML();
        var prompt = test.Context("This is a unittest. Will the asserts fail or not?");
        var stopWords = test.StopWords;
        Assert.NotNull(prompt);
        Assert.Equal("<|im_start|>system\nYou are unittest\nYour purpose is to test ChatMLCreator\n<|im_end|><|im_start|>context\nThis is a unittest. Will the asserts fail or not?<|im_end|><|im_start|>unittest\n", prompt);
        Assert.NotEmpty(stopWords);
        Assert.Contains("<|im_start|>", stopWords);
        Assert.Contains("<|im_end|>", stopWords);
    }

    [Fact]
    public void Chat()
    {
        var test = new TestChatML();
        var prompt = test.Chat("this is unit test");
        var stopWords = test.StopWords;
        Assert.NotNull(prompt);
        Assert.Equal("<|im_start|>system\nYou are unittest\nYour purpose is to test ChatMLCreator\n<|im_end|><|im_start|>user\nthis is unit test<|im_end|><|im_start|>unittest\n", prompt);
        Assert.NotEmpty(stopWords);
        Assert.Contains("<|im_start|>", stopWords);
        Assert.Contains("<|im_end|>", stopWords);
    }

    [Fact]
    public void ChatWithHistory()
    {
        var test = new TestChatML();
        var history = new AiChatHistoryEntry[] {
            new()
            {
                Input = "this is unit test",
                Output = "yes you are right"
            },
            new()
            {
                Input = "Are you always working correctly?",
                Output = "for sure I am not"
            }
        };
        var prompt = test.Chat("Next message", history);
        var stopWords = test.StopWords;
        Assert.NotNull(prompt);
        Assert.Equal("<|im_start|>system\nYou are unittest\nYour purpose is to test ChatMLCreator\n<|im_end|><|im_start|>user\nthis is unit test<|im_end|><|im_start|>unittest\nyes you are right<|im_end|><|im_start|>user\nAre you always working correctly?<|im_end|><|im_start|>unittest\nfor sure I am not<|im_end|><|im_start|>user\nNext message<|im_end|><|im_start|>unittest\n", prompt);
        Assert.NotEmpty(stopWords);
        Assert.Contains("<|im_start|>", stopWords);
        Assert.Contains("<|im_end|>", stopWords);
    }
}
