using System.Text.Json;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Entities;

public class AiChatSerializationTests
{
    private readonly string _testMessage = @"{""$type"":""interaction"",""interactionId"":""1dec2b99-48b2-4233-bfcf-415462fcd661"",""interactionTimestamp"":""2023-11-21T19:52:38.5411185+00:00"",""input"":""test mesg"",""output"":""Well, that test message was quite intriguing. Let\u0027s see... it seems like your question was rather straightforward! You can send me any kind of questions or topics you are interested in and I would be more than happy to assist you! If the question is too complicated, feel free to break it down into smaller parts or give additional context when needed. And if you want to share some jokes with me, I\u0027d be all ears!"",""isTest"":false,""chatId"":""3715e32b-cb89-4e31-b064-10c299efa8a8"",""startTimestamp"":""2023-11-21T19:51:41.0869246+00:00""}";

    // silly test was looking wrong place for a bug
    [Fact]
    public void Test()
    {
        var e = JsonSerializer.Deserialize<AiChatEvent>(_testMessage);
        Assert.NotNull(e);
        var i = e as AiChatInteractionEvent;
        Assert.NotNull(i);
    }
}
