using System.Text;
using AJE.Domain.Ai;
using AJE.Domain.Entities;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Infra;

/// <summary>
/// Tests require llama.cpp
/// </summary>
[Collection("Llama")]
public class LlamaAiModelTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>, IClassFixture<LlamaQueueFixture>
{
    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;
    private readonly LlamaQueueFixture _llamaQueueFixture;

    public LlamaAiModelTests(
        HttpClientFixture fixture,
        RedisFixture redisFixture,
        LlamaQueueFixture llamaQueueFixture)
    {
        _httpClientFixture = fixture;
        _redisFixture = redisFixture;
        _llamaQueueFixture = llamaQueueFixture;
    }

    private IServiceProvider CreateMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LlamaApi>))).Returns(new Mock<ILogger<LlamaApi>>().Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IHttpClientFactory))).Returns(_httpClientFixture.HttpClientFactory);
        return mockServiceProvider.Object;
    }

    [Fact]
    public async Task CompletionAsyncTest()
    {
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
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
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
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
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
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
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
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
        var model = new LlamaAiModel(new Mock<ILogger<LlamaAiModel>>().Object, CreateMockServiceProvider(), TestConstants.LlamaConfiguration, _redisFixture.Connection, true);
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
