using System.Text.Json;
using AJE.Domain.Entities;

namespace AJE.Test.Unit.Domain.Entities;

// useless silly tests
public class AiChatSerializationTests
{
    [Fact]
    public void AiChatOptionsTest()
    {
        var json = @"{
            ""chatId"": ""00000000-1000-0000-0000-000000000001""
        }";
        var options = JsonSerializer.Deserialize<AiChatOptions>(json);
        Assert.NotNull(options);
    }
    [Fact]
    public void AiChatTest()
    {
        var json = @"{
            ""chatId"": ""00000000-1000-0000-0000-000000000001"",
            ""startTimestamp"": ""2021-09-12T12:00:00+00:00"",
            ""interactions"": [
                {
                    ""interactionId"": ""00000000-1000-0000-0000-000000000002"",
                    ""interactionTimestamp"": ""2021-09-12T12:00:00+00:00"",
                    ""input"": ""Hello"",
                    ""output"": ""Hey stranger, how can I help you?"",
                    ""model"": ""llama-unit-test"",
                    ""numberOfTokensEvaluated"": 24,
                    ""numberOfTokensContext"": 1024
                }
            ]
        }";
        var chat = JsonSerializer.Deserialize<AiChat>(json);
        Assert.NotNull(chat);
        Assert.Single(chat.Interactions);
    }

    [Fact]
    public void AiChatInteractionEntryTest()
    {
        var json = @"{
            ""interactionId"": ""00000000-1000-0000-0000-000000000002"",
            ""interactionTimestamp"": ""2021-09-12T12:00:00+00:00"",
            ""input"": ""Hello"",
            ""output"": ""Hey stranger, how can I help you?"",
            ""model"": ""llama-unit-test"",
            ""numberOfTokensEvaluated"": 24,
            ""numberOfTokensContext"": 1024
        }";
        var entry = JsonSerializer.Deserialize<AiChatInteractionEntry>(json);
        Assert.NotNull(entry);
    }
}
