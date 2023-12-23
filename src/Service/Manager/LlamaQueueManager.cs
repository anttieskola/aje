namespace AJE.Service.Manager;

/// <summary>
/// Queue manager that works directly witht he resource events
/// Still had issue where there was queue but no user after a analyezer crash
/// The Cleanup is not working as expected, added ActivateIfFree to try to fix
/// After Cleanup resource got stuck in free state
/// </summary>
public class LlamaQueueManager : BackgroundService
{
    private readonly ILogger<LlamaQueueManager> _logger;
    private readonly IConnectionMultiplexer _connection;
    private readonly LlamaConfiguration _configuration;
    private readonly RedisChannel _channel;
    private readonly bool _isTest;
    private int _cleanupAfterMinutes = 3;

    public LlamaQueueManager(
        ILogger<LlamaQueueManager> logger,
        IConnectionMultiplexer connection,
        LlamaConfiguration configuration,
        bool isTestMode = false)
    {
        _logger = logger;
        _connection = connection;
        _configuration = configuration;
        _channel = new RedisChannel(ResourceEventChannels.LlamaAi, RedisChannel.PatternMode.Auto);
        _isTest = isTestMode;
    }

    private readonly ConcurrentDictionary<string, ResourceQueue> _resources = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // add resources
        foreach (var server in _configuration.Servers)
        {
            var management = new ResourceQueue(server.ResourceName);
            if (!_resources.TryAdd(server.ResourceName, management))
            {
                throw new PlatformException("Failed to add resource management");
            }
        }

        // sub to messages
        _connection.GetSubscriber().Subscribe(_channel, OnMessage);

        // loop
        while (!stoppingToken.IsCancellationRequested)
        {
            Statistics();
            Cleanup();
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            ActivateIfFree();
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }

    private void OnMessage(RedisChannel channel, RedisValue message)
    {
        if (!message.HasValue)
            return;

        var resourceEvent = JsonSerializer.Deserialize<ResourceEvent>(message.ToString());
        if (resourceEvent is ResourceRequestEvent request && request.IsTest == _isTest)
        {
            HandleRequest(request);

        }
        else if (resourceEvent is ResourceReleasedEvent released && released.IsTest == _isTest)
        {
            HandleReleased(released);
        }
    }

    private void HandleRequest(ResourceRequestEvent request)
    {
        if (_resources.TryGetValue(request.ResourceName, out var management))
        {
            management.Request(request);
            if (management.IsFree())
            {
                var grant = management.GetNext();
                if (grant != null)
                    Publish(grant);
            }
        }
        else
        {
            throw new PlatformException($"Failed to find resource management for {request.ResourceName}");
        }
    }

    private void HandleReleased(ResourceReleasedEvent released)
    {
        if (_resources.TryGetValue(released.ResourceName, out var management))
        {
            management.Release(released);
            if (management.IsFree()) // gotta check as someone might give up queue position
            {
                var grant = management.GetNext();
                if (grant != null)
                    Publish(grant);
            }
        }
        else
        {
            throw new PlatformException($"Failed to find resource management for {released.ResourceName}");
        }
    }

    private void Publish(ResourceEvent resourceEvent)
    {
        resourceEvent.IsTest = _isTest;
        var publisher = _connection.GetSubscriber();
        publisher.Publish(_channel, JsonSerializer.Serialize(resourceEvent));
    }

    private void Statistics()
    {
        foreach (var resource in _resources)
        {
            _logger.LogInformation("Resource:{resourceName}\tTotal:{TotalCount}\tQueue:{QueueCount}\tActive:{Current}", resource.Key, resource.Value.TotalCount(), resource.Value.QueueCount(), resource.Value.Current() == Guid.Empty ? "None/Free" : resource.Value.Current());
        }
    }

    private void Cleanup()
    {
        foreach (var resource in _resources)
        {
            var releaseEvent = resource.Value.Cleanup(_cleanupAfterMinutes);
            if (releaseEvent != null)
            {
                _cleanupAfterMinutes += 2;
                Publish(releaseEvent);
            }
            else
            {
                _cleanupAfterMinutes = 3;
            }
        }
    }

    private void ActivateIfFree()
    {
        foreach (var resource in _resources)
        {
            if (resource.Value.IsFree())
            {
                var grant = resource.Value.GetNext();
                if (grant != null)
                    Publish(grant);
            }
        }
    }
}
