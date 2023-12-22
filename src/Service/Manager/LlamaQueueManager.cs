namespace AJE.Service.Manager;

/// <summary>
/// Nostin LlamaAPI:Sta odotus ajan 3-6 sec x 10, serveri oli jumissa jostain syystä...
/// riittäisö min 30 sekunttia et se toimisi taas?
///
/// Ongelma oli että serveri oli busy, threadi kaatui ja koko analyysi sovellus.
/// Kaatumisessa se vapautti resurssin, manageri varmaan lähetti seuraavalle käyttäjälle
/// joka oli samasssa sovelluksessa toinen threadi mutta koska sekiin kaatui niin sitä ei vapautettu.
///
// Dec 20 01:09:56 zeus AJE.Service.Manager[202773]: info: AJE.Service.Manager.LlamaQueueManager[0] Resource:llama-apollo        Total:380        Queue:2        Active:16b4bfef-935c-4009-9344-3aa7b513910c
// NewsAnalyzer kaatuu
// Dec 20 01:03:13 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 10/10 after 00:00:01.8840000 ms
// Dec 20 01:03:11 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 9/10 after 00:00:01.9820000 ms
// Dec 20 01:03:10 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 8/10 after 00:00:00.8460000 ms
// Dec 20 01:03:10 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 7/10 after 00:00:00.1900000 ms
// Dec 20 01:03:08 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 6/10 after 00:00:01.8510000 ms
// Dec 20 01:03:06 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 5/10 after 00:00:01.9620000 ms
// Dec 20 01:03:06 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 4/10 after 00:00:00.4280000 ms
// Dec 20 01:03:04 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 3/10 after 00:00:01.6480000 ms
// Dec 20 01:03:03 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 2/10 after 00:00:00.7290000 ms
// Dec 20 01:03:01 zeus AJE.Service.NewsAnalyzer[202892]: warn: AJE.Infra.Ai.LlamaApi[0] Server is busy, retrying 1/10 after 00:00:01.7830000 ms
// Dec 20 01:02:43 zeus AJE.Service.NewsAnalyzer[202892]: info: AJE.Infra.Ai.LlamaAiModel[0] Wait        llama-apollo        16b4bfef-935c-4009-9344-3aa7b513910c
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
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            Statistics();
            Cleanup();
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
}
