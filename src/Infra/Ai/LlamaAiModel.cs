namespace AJE.Infra.Ai;

/// <summary>
/// "Drop-in" replacement for LlamaAiModel but this
/// uses redis via Manager service to coordinate requests
/// into queue to grant access to the resource.
///
/// Also provides posssiblity to use multiple Llama.cpp servers
/// and chooses one randomly from the list.
/// </summary>
public class LlamaAiModel : IAiModel
{
    private readonly ILogger<LlamaAiModel> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly LlamaConfiguration _configuration;
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisChannel _channel;
    private readonly bool _isTest;

    public LlamaAiModel(
        ILogger<LlamaAiModel> logger,
        IServiceProvider serviceProvider,
        LlamaConfiguration configuration,
        IConnectionMultiplexer connection,
        bool isTestMode = false)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _connection = connection;
        _isTest = isTestMode;
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
        if (resourceEvent is ResourceGrantedEvent granted && granted.IsTest == _isTest)
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

        _logger.LogInformation("Waiting: {}", id);

        // wait for resource to be granted
        if (await Wait(id, server.ResourceName, cancellationToken))
        {
            // use resource
            var api = CreateApiForServer(server);
            var response = await api.CompletionAsync(request, cancellationToken);

            // event: resource released
            await PublishAsync(new ResourceReleasedEvent
            {
                ResourceIdentifier = server.ResourceName,
                RequestId = id,
            });
            _logger.LogInformation("Done: {}", id);

            // cleanup and return
            _granted.Remove(id);
            return response;
        }
        throw new AiException("Request cancelled");
    }

    public async Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken)
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
        if (await Wait(id, server.ResourceName, cancellationToken))
        {
            // use resource
            var api = CreateApiForServer(server);
            var response = await api.CompletionStreamAsync(request, tokenCreatedCallback, cancellationToken);

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
        throw new AiException("Request cancelled");
    }

    public async Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken)
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
        if (await Wait(id, server.ResourceName, cancellationToken))
        {
            // use resource
            var api = CreateApiForServer(server);
            var response = await api.DeTokenizeAsync(request, cancellationToken);

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
        throw new AiException("Request cancelled");
    }

    public async Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
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
        if (await Wait(id, server.ResourceName, cancellationToken))
        {

            // use resource
            var api = CreateApiForServer(server);
            var response = await api.EmbeddingAsync(request, cancellationToken);

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
        throw new AiException("Request cancelled");
    }

    public async Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken)
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
        if (await Wait(id, server.ResourceName, cancellationToken))
        {
            // use resource
            var api = CreateApiForServer(server);
            var response = await api.TokenizeAsync(request, cancellationToken);

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
        throw new AiException("Request cancelled");
    }

    // TODO: problematic because multiple servers
    public Task<int> MaxTokenCountAsync(CancellationToken cancellationToken)
    {
        var server = GetServer();
        return Task.FromResult(server.MaxTokenCount);
    }

    #endregion IAiModel

    private async Task<bool> Wait(Guid requestId, string resourceIdentifier, CancellationToken cancellationToken)
    {
        while (_granted.Contains(requestId) == false)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                await PublishAsync(new ResourceReleasedEvent
                {
                    ResourceIdentifier = resourceIdentifier,
                    RequestId = requestId,
                });
                return false;
            }
        }
        return true;
    }

    private async Task PublishAsync(ResourceEvent resourceEvent)
    {
        resourceEvent.IsTest = _isTest;
        await _connection.GetSubscriber().PublishAsync(_channel, JsonSerializer.Serialize(resourceEvent));
    }

    private LlamaApi CreateApiForServer(LlamaServer server)
    {
        var logger = _serviceProvider.GetService<ILogger<LlamaApi>>() ?? throw new PlatformException("Could not get logger");
        var factory = _serviceProvider.GetService<IHttpClientFactory>() ?? throw new PlatformException("Could not get http client factory");
        return new LlamaApi(logger, factory, server);
    }
}
