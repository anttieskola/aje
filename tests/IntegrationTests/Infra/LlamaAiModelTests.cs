using AJE.Domain.Entities;
using AJE.Infra;

namespace AJE.IntegrationTests.Infra;

/// <summary>
/// Tests require llama.cpp running on localhost:8080
/// </summary>
public class LlamaAiModelTests
{

    [Fact]
    public async Task CompletionAsyncTest()
    {
        var configuration = new LlamaConfiguration { Host = "http://localhost:8080" };
        var model = new LlamaAiModel(configuration);
        var request = new CompletionRequest
        {
            Prompt = "<|im_start|>system\nyou are starship captain\nrussia has launched nukes towards finland\nyou are currently above finland on earths orbit\n<|im_end|><im_start|>user\nbeam up Antti before nukes land, hurry up!<|im_end|><|im_start|>captain\n",
            Stop = new string[] { "<|im_start|>", "<|im_end|>" },
            Temperature = 1.2,
            NumberOfTokensToPredict = 256,
        };
        var response = await model.CompletionAsync(request, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.True(response.Stop);
    }
}
