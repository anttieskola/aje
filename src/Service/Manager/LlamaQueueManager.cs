using System.Collections.Concurrent;

namespace AJE.Service.Manager;

public class LlamaQueueManager : BackgroundService
{
    private readonly ILogger<LlamaQueueManager> _logger;
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisChannel _channel;
    private readonly bool _isTest;

    public LlamaQueueManager(
        ILogger<LlamaQueueManager> logger,
        IConnectionMultiplexer connection,
        bool isTestMode = false)
    {
        _logger = logger;
        _connection = connection;
        _channel = new RedisChannel(ResourceEventChannels.LlamaAi, RedisChannel.PatternMode.Auto);
        _isTest = isTestMode;
    }

    private ConcurrentDictionary<string, DateTimeOffset> _resources = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection.GetSubscriber().Subscribe(_channel, OnMessage);
        while (!stoppingToken.IsCancellationRequested)
        {
            Statistics();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private void Statistics()
    {
        foreach (var resource in _resources)
        {
            var db = _connection.GetDatabase();
            var length = db.ListLength(resource.Key);
            _logger.LogInformation("Resource {ResourceIdentifier} has {Length} requests", resource, length);
        }
    }

    private void OnMessage(RedisChannel channel, RedisValue message)
    {
        if (!message.HasValue)
            return;

        var resourceEvent = JsonSerializer.Deserialize<ResourceEvent>(message.ToString());
        if (resourceEvent is ResourceRequestEvent request && request.IsTest == _isTest)
        {
            if (!_resources.ContainsKey(request.ResourceIdentifier))
            {
                _resources.TryAdd(request.ResourceIdentifier, DateTimeOffset.UtcNow);
                _logger.LogInformation("Resource {ResourceIdentifier} added", request.ResourceIdentifier);
            }

            HandleRequest(request);

        }
        else if (resourceEvent is ResourceReleasedEvent released && released.IsTest == _isTest)
        {
            HandleReleased(released);
        }
    }

    private void HandleReleased(ResourceReleasedEvent released)
    {
        var db = _connection.GetDatabase();
        var length = db.ListLength(released.ResourceIdentifier);
        if (length > 0)
        {
            // after testing due messages arriving order is not guranteed to be correct
            // we can push left when resource reserved in wrong order
            _logger.LogInformation("Resource {ResourceIdentifier} released from id {}", released.ResourceIdentifier, released.RequestId);
            db.ListRemove(released.ResourceIdentifier, released.RequestId.ToString());
            length = db.ListLength(released.ResourceIdentifier);
            if (length > 0)
            {
                var nextId = db.ListGetByIndex(released.ResourceIdentifier, length - 1);
                var nextIdGuid = Guid.ParseExact(nextId.ToString(), "D");
                _logger.LogInformation("Resource {ResourceIdentifier} granted to id {RequestId}", released.ResourceIdentifier, nextIdGuid);
                Publish(new ResourceGrantedEvent
                {
                    ResourceIdentifier = released.ResourceIdentifier,
                    RequestId = nextIdGuid,
                });
            }
        }
        else
        {
            _logger.LogWarning("Resource {ResourceIdentifier} released but no requests pending", released.ResourceIdentifier);
        }
    }

    private void HandleRequest(ResourceRequestEvent request)
    {
        var db = _connection.GetDatabase();
        var length = db.ListLeftPush(request.ResourceIdentifier, request.RequestId.ToString());
        if (length == 1)
        {
            _logger.LogInformation("Resource {ResourceIdentifier} granted to id {RequestId}", request.ResourceIdentifier, request.RequestId);
            Publish(new ResourceGrantedEvent
            {
                ResourceIdentifier = request.ResourceIdentifier,
                RequestId = request.RequestId,
            });
        }
    }

    private void Publish(ResourceEvent resourceEvent)
    {
        resourceEvent.IsTest = _isTest;
        var publisher = _connection.GetSubscriber();
        publisher.Publish(_channel, JsonSerializer.Serialize(resourceEvent));
    }
}
