using System.Collections.Concurrent;
using AJE.Domain.Entities;

namespace AJE.Service.Manager;

public class LlamaQueueManager : BackgroundService
{
    private readonly ILogger<LlamaQueueManager> _logger;
    private readonly IConnectionMultiplexer _connection;
    private readonly LlamaConfiguration _configuration;
    private readonly RedisChannel _channel;
    private readonly bool _isTest;

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

    private readonly ConcurrentDictionary<Guid, ResourceRequest> _requests = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // clean queues
        foreach (var server in _configuration.Servers)
        {
            var db = _connection.GetDatabase();
            var lenght = db.ListLength(server.ResourceName);
            for (int i = 0; i < lenght; i++)
            {
                db.ListRightPop(server.ResourceName);
            }
        }

        // sub to messages
        _connection.GetSubscriber().Subscribe(_channel, OnMessage);

        // loop
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            Statistics();
            Cleanup();
        }
    }

    private void Statistics()
    {
        foreach (var server in _configuration.Servers)
        {
            var db = _connection.GetDatabase();
            var length = db.ListLength(server.ResourceName);
            _logger.LogInformation("Resource {ResourceIdentifier} has queue lenght {Length}", server.ResourceName, length);
        }
    }

    private void Cleanup()
    {
        foreach (var request in _requests)
        {
            if (request.Value.Time.AddMinutes(10) < DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Use of resource {ResourceName} with id {Key} has been running over 10 minutes releasing...", request.Value.ResourceName, request.Key);
                Publish(new ResourceReleasedEvent
                {
                    ResourceIdentifier = request.Value.ResourceName,
                    RequestId = request.Key,
                });
            }
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

        // remove from requests
        if (!_requests.TryRemove(released.RequestId, out _))
        {
            _logger.LogCritical("Could not remove usage of {ResourceIdentifier} with id {RequestId} from request cache", released.ResourceIdentifier, released.RequestId);
        }
        else
        {
            foreach (var request in _requests)
            {
                _logger.LogCritical("(REMOVE) Pending in cache: {ResourceName} with id {Key}", request.Value.ResourceName, request.Key);
            }
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

        // add to requests
        if (!_requests.TryAdd(request.RequestId, new ResourceRequest
        {
            ResourceName = request.ResourceIdentifier,
            RequestId = request.RequestId,
            Time = DateTimeOffset.UtcNow,
        }))
        {
            _logger.LogCritical("Could not add usage of {ResourceIdentifier} with id {RequestId} to request cache", request.ResourceIdentifier, request.RequestId);
        }
        else
        {
            foreach (var req in _requests)
            {
                _logger.LogCritical("(ADD) Pending in cache: {ResourceName} with id {Key}", req.Value.ResourceName, req.Key);
            }
        }
    }

    private void Publish(ResourceEvent resourceEvent)
    {
        resourceEvent.IsTest = _isTest;
        var publisher = _connection.GetSubscriber();
        publisher.Publish(_channel, JsonSerializer.Serialize(resourceEvent));
    }
}
