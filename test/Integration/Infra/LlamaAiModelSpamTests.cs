using AJE.Domain.Entities;
using AJE.Infra.Ai;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Infra;

/// <summary>
/// Tests requiring a running Llama instance
/// (When all tests are run these methods are ran in parallel)
/// </summary>
[Collection("Llama")]
public class LlamaApiSpamTests : IClassFixture<HttpClientFixture>
{
    private readonly HttpClientFixture _fixture;
    private readonly LlamaServer _server;
    private readonly string _randomText;
    private readonly string _completionPrompt;
    private readonly string[] _stopWords;

    public LlamaApiSpamTests(HttpClientFixture fixture)
    {
        _fixture = fixture;
        _server = new LlamaServer { Host = TestConstants.LlamaAddress, ResourceName = "integration-spam-test", MaxTokenCount = 8096, TimeoutInSeconds = 30 };
        _randomText = TestConstants.GenerateRandomString(128);
        _completionPrompt = "<|im_start|>system\nYou are a very funny assistant that writes long funny stories about the world and the universe.\n<|im_end|><|im_start|>userOh noes I just arrived to this planet filled with angry people...\n<|im_end|><|im_start|>assistant\n";
        _stopWords = ["<|im_start|>", "<|im_end|>"];
    }

    #region completion
    [Fact]
    public async Task Completion_1()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Completion_2()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Completion_3()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Completion_4()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Completion_5()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords }, CancellationToken.None);
        Assert.NotNull(response);
    }
    #endregion completion

    #region completion stream
    static Task TokenCallBack(string token) => Task.CompletedTask;
    [Fact]
    public async Task CompletionStream_1()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionStreamAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords, Stream = true }, TokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task CompletionStream_2()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionStreamAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords, Stream = true }, TokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task CompletionStream_3()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionStreamAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords, Stream = true }, TokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task CompletionStream_4()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionStreamAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords, Stream = true }, TokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task CompletionStream_5()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.CompletionStreamAsync(new CompletionRequest { Prompt = _completionPrompt, Temperature = 1.0, TopK = 100, NumberOfTokensToPredict = 32, Stop = _stopWords, Stream = true }, TokenCallBack, CancellationToken.None);
        Assert.NotNull(response);
    }
    #endregion completion stream

    #region Embeddings
    [Fact]
    public async Task Embedding_1()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.EmbeddingAsync(new EmbeddingRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Embedding_2()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.EmbeddingAsync(new EmbeddingRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Embedding_3()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.EmbeddingAsync(new EmbeddingRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Embedding_4()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.EmbeddingAsync(new EmbeddingRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Embedding_5()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var embeddingResponse = await model.EmbeddingAsync(new EmbeddingRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(embeddingResponse);
    }
    #endregion Embeddings

    #region Tokenize
    [Fact]
    public async Task Tokenize_1()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.TokenizeAsync(new TokenizeRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Tokenize_2()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.TokenizeAsync(new TokenizeRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Tokenize_3()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.TokenizeAsync(new TokenizeRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Tokenize_4()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.TokenizeAsync(new TokenizeRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task Tokenize_5()
    {
        var model = new LlamaApi(new Mock<ILogger<LlamaApi>>().Object, _fixture.HttpClientFactory, _server);
        var response = await model.TokenizeAsync(new TokenizeRequest { Content = _randomText }, CancellationToken.None);
        Assert.NotNull(response);
    }
    #endregion Tokenize
}
