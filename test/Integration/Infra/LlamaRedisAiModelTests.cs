using AJE.Domain.Entities;
using AJE.Infra.AiRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

#pragma warning disable xUnit1033
public class LlamaRedisAiModelTests : IClassFixture<HttpClientFixture>, IClassFixture<RedisFixture>
{
#pragma warning restore xUnit1033

    private readonly HttpClientFixture _httpClientFixture;
    private readonly RedisFixture _redisFixture;

    public LlamaRedisAiModelTests(
        HttpClientFixture httpClientFixture,
        RedisFixture redisFixture)
    {
        _httpClientFixture = httpClientFixture;
        _redisFixture = redisFixture;
    }

    private IServiceProvider CreateMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(x => x.GetService(typeof(ILogger<LlamaApi>))).Returns(new Mock<ILogger<LlamaApi>>().Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IHttpClientFactory))).Returns(_httpClientFixture.HttpClientFactory);
        return mockServiceProvider.Object;
    }

    // TODO: This and others need fixing too, they gotta run the manager in background
    [Fact(Skip = "This tested manually to work (send message manually to grant resource)")]
    public async Task Completion()
    {
        var configuration = new LlamaConfiguration
        {
            Servers =
            [
                new LlamaServer
                {
                    ResourceName = "llama-localhost",
                    Host = "http://localhost:8080",
                    MaxTokenCount = 16384,
                    TimeoutInSeconds = 3600,
                }
            ]
             ,
            LogFolder = "/var/aje/ai"
        };
        var model = new LlamaRedisAiModel(CreateMockServiceProvider(), configuration, _redisFixture.Connection);
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
