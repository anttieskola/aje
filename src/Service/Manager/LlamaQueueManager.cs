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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection.GetSubscriber().Subscribe(_channel, OnMessage);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
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
            var last = db.ListGetByIndex(released.ResourceIdentifier, length - 1);
            if (last.ToString() == released.RequestId.ToString())
            {
                db.ListRightPop(released.ResourceIdentifier);
                length = db.ListLength(released.ResourceIdentifier);
                if (length > 0)
                {
                    var nextId = db.ListGetByIndex(released.ResourceIdentifier, length - 1);
                    Publish(new ResourceGrantedEvent
                    {
                        ResourceIdentifier = released.ResourceIdentifier,
                        RequestId = Guid.ParseExact(nextId.ToString(), "D"),
                    });
                }
            }
            else
            {
                _logger.LogWarning("Resource {ResourceIdentifier} released but not last request, requestId:{RequestId}", released.ResourceIdentifier, released.RequestId);
                db.ListRemove(released.ResourceIdentifier, released.RequestId.ToString());
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
