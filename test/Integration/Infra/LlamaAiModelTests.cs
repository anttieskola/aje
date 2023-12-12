using System.Text;
using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Infra;

/// <summary>
/// Tests require llama.cpp
/// </summary>
public class LlamaAiModelTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClientFixture _fixture;

    public LlamaAiModelTests(HttpClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CompletionAsyncTest()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/var/aje/ai" };
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory);
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

    [Fact]
    public async Task CompletionStreamAsync()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/var/aje/ai" };
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory);
        var request = new CompletionRequest
        {
            Prompt = "<|im_start|>system\nyou are starship captain\nrussia has launched nukes towards finland\nyou are currently above finland on earths orbit\n<|im_end|><im_start|>user\nbeam up Antti before nukes land, hurry up!<|im_end|><|im_start|>captain\n",
            Stop = new string[] { "<|im_start|>", "<|im_end|>" },
            Temperature = 1.2,
            NumberOfTokensToPredict = 256,
            Stream = true,
        };
        var tokens = new StringBuilder();
        Task tokenCallBack(string token)
        {
            tokens.Append(token);
            return Task.CompletedTask;
        }
        var response = await model.CompletionStreamAsync(request, tokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
        Assert.True(response.Stop);
        Assert.NotEmpty(tokens.ToString());
        Assert.Equal(response.Content, tokens.ToString());
    }

    [Fact]
    public async Task TokenizeAndDeTokenize()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/var/aje/ai" };
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory);
        var tokenizeRequest = new TokenizeRequest
        {
            Content = "Antti",
        };
        var tokenizeResponse = await model.TokenizeAsync(tokenizeRequest, CancellationToken.None);
        Assert.NotNull(tokenizeResponse);
        Assert.NotEmpty(tokenizeResponse.Tokens);

        var detokenizeRequest = new DeTokenizeRequest
        {
            Tokens = tokenizeResponse.Tokens,
        };
        var detokenizeResponse = await model.DeTokenizeAsync(detokenizeRequest, CancellationToken.None);
        Assert.NotNull(detokenizeResponse);
        Assert.NotEmpty(detokenizeResponse.Content);
        Assert.Equal(tokenizeRequest.Content, detokenizeResponse.Content.Trim());
    }

    [Fact]
    public async Task Embedding()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/var/aje/ai" };
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory);
        var embeddingRequest = new EmbeddingRequest
        {
            Content = "Antti",
        };
        var embeddingResponse = await model.EmbeddingAsync(embeddingRequest, CancellationToken.None);
        Assert.NotNull(embeddingResponse);
        Assert.NotEmpty(embeddingResponse.Embedding);
    }

    [Fact]
    public async Task AnttiChatMLCreatorPromptLength()
    {
        var configuration = new LlamaConfiguration { Host = TestConstants.LlamaAddress, LogFolder = "/var/aje/ai" };
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, configuration, _fixture.HttpClientFactory);
        var a = new AntaiChatML();
        var tokenizeRequest = new TokenizeRequest
        {
            Content = a.Context(string.Empty),
        };
        var tokenizeResponse = await model.TokenizeAsync(tokenizeRequest, CancellationToken.None);
        var length = tokenizeResponse.Tokens.Length;
        // One news article was 14087 tokens long, to classify it took half'n hour
        Assert.True(length < 9000);
    }
}
