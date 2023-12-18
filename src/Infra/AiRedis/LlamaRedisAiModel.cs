namespace AJE.Infra.AiRedis;

/// <summary>
/// "Drop-in" replacement for LlamaAiModel but this
/// uses redis via Manager service to coordinate requests
/// into queue to grant access to the resource.
///
/// Also provides posssiblity to use multiple Llama.cpp servers
/// and chooses one randomly from the list.
/// </summary>
public class LlamaRedisAiModel : IAiModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly LlamaConfiguration _configuration;
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisChannel _channel;

    public LlamaRedisAiModel(
        IServiceProvider serviceProvider,
        LlamaConfiguration configuration,
        IConnectionMultiplexer connection)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _connection = connection;
        _channel = new RedisChannel(ResourceEventChannels.LlamaAi, RedisChannel.PatternMode.Auto);

        var subscriber = _connection.GetSubscriber();
        subscriber.Subscribe(_channel, OnMessage);
    }

    private LlamaServer GetServer()
    {
        var count = _configuration.Servers.Length;
        if (count == 0)
        {
            throw new AiException("No servers configured");
        }
        var index = new Random().Next(0, count);
        return _configuration.Servers[index];
    }

    private readonly List<Guid> _granted = [];

    private void OnMessage(RedisChannel channel, RedisValue message)
    {
        if (!message.HasValue)
            return;

        var resourceEvent = JsonSerializer.Deserialize<ResourceEvent>(message.ToString());
        if (resourceEvent is ResourceGrantedEvent granted)
        {
            _granted.Add(granted.RequestId);
        }
    }

    #region IAiModel

    public async Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        // get random server
        var server = GetServer();

        // request id
        var id = Guid.NewGuid();

        // event: resource requested
        await PublishAsync(new ResourceRequestEvent
        {
            ResourceIdentifier = server.ResourceName,
            RequestId = id,
        });

        // wait for resource to be granted
        await Wait(id, cancellationToken);

        // use resource
        var api = CreateApiForServer(server);
        var response = await api.CompletionAsync(request, cancellationToken);

        // event: resource released
        await PublishAsync(new ResourceReleasedEvent
        {
            ResourceIdentifier = server.ResourceName,
            RequestId = id,
        });

        // cleanup and return
        _granted.Remove(id);
        return response;
    }

    public Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> MaxTokenCountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #endregion IAiModel

    private async Task Wait(Guid requestId, CancellationToken cancellationToken)
    {
        while (_granted.Contains(requestId) == false)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }
    }

    private async Task PublishAsync(ResourceEvent resourceEvent)
    {
        await _connection.GetSubscriber().PublishAsync(_channel, JsonSerializer.Serialize(resourceEvent));
    }

    private LlamaApi CreateApiForServer(LlamaServer server)
    {
        var logger = _serviceProvider.GetService<ILogger<LlamaApi>>() ?? throw new PlatformException("Could not get logger");
        var factory = _serviceProvider.GetService<IHttpClientFactory>() ?? throw new PlatformException("Could not get http client factory");
        return new LlamaApi(logger, factory, server);
    }
}
