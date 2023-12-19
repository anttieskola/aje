using System.Text.Json;
using AJE.Domain.Events;
using AJE.Service.Manager;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration.Service.Manager;

public class LlamaQueueManagerTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _redisFixture;
    private readonly RedisChannel _channel;
    private const string _resourceIdentifier = "llama-integration-test-queuemanager";
    public LlamaQueueManagerTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
        _channel = new RedisChannel(ResourceEventChannels.LlamaAi, RedisChannel.PatternMode.Auto);
    }

    /// <summary>
    /// Test the lifecycle of the Llama queue manager
    /// Makes multiple requests, releases them, and ensures they are granted in the correct order
    /// This is pretty hard to debug as multiple threads are involved, so usually debugging results in test failure
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task LifeCycle()
    {
        var configuration = TestConstants.LlamaConfiguration;
        configuration.Servers[0].ResourceName = _resourceIdentifier;

        var manager = new LlamaQueueManager(
            new Mock<ILogger<LlamaQueueManager>>().Object,
             _redisFixture.Connection, configuration, true);

        var requestOneId = Guid.NewGuid();
        var requestTwoId = Guid.NewGuid();
        var requestThreeId = Guid.NewGuid();
        var requestIds = new List<Guid> { requestOneId, requestTwoId, requestThreeId };

        var cancellationTokenSource = new CancellationTokenSource();
        var task = Task.Factory.StartNew(() =>
        {
            manager.StartAsync(cancellationTokenSource.Token);
        });
        await Task.Delay(50);

        var grantedMessages = new List<ResourceGrantedEvent>();
        _redisFixture.Connection.GetSubscriber().Subscribe(_channel, (channel, message) =>
        {
            if (!message.HasValue)
                return;

            var resourceEvent = JsonSerializer.Deserialize<ResourceEvent>(message.ToString());
            if (resourceEvent == null || !resourceEvent.IsTest)
                return;

            var granted = resourceEvent as ResourceGrantedEvent;
            if (granted != null)
            {
                if (requestIds.Contains(granted.RequestId))
                {
                    if (!grantedMessages.Any(x => x.RequestId == granted.RequestId))
                        grantedMessages.Add(granted);
                }
            }
        });

        // Act create first request

        Publish(new ResourceRequestEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestOneId,
        });
        await Task.Delay(50);
        Assert.Single(grantedMessages);
        {
            var granted = grantedMessages[0] as ResourceGrantedEvent;
            Assert.NotNull(granted);
            Assert.Equal(_resourceIdentifier, granted.ResourceName);
            Assert.Equal(requestOneId, granted.RequestId);
        }
        // Act create second request
        Publish(new ResourceRequestEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestTwoId,
        });
        // should be in queue, not granted
        await Task.Delay(50);
        Assert.Single(grantedMessages);

        // Act create third request
        Publish(new ResourceRequestEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestThreeId,
        });
        // should be in queue, not granted
        await Task.Delay(50);
        Assert.Single(grantedMessages);

        // odd case second one gives up
        Publish(new ResourceReleasedEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestTwoId,
        });
        await Task.Delay(50);

        // first one releases
        Publish(new ResourceReleasedEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestOneId,
        });
        await Task.Delay(50);
        Assert.Equal(2, grantedMessages.Count);
        {
            var granted = grantedMessages[1] as ResourceGrantedEvent;
            Assert.NotNull(granted);
            Assert.Equal(_resourceIdentifier, granted.ResourceName);
            Assert.Equal(requestThreeId, granted.RequestId);
        }
        await Task.Delay(50);

        // third one releases
        Publish(new ResourceReleasedEvent
        {
            ResourceName = _resourceIdentifier,
            RequestId = requestThreeId,
        });

        // cleanup
        cancellationTokenSource.Cancel();
    }

    private void Publish(ResourceEvent resourceEvent)
    {
        resourceEvent.IsTest = true;
        var publisher = _redisFixture.Connection.GetSubscriber();
        publisher.Publish(_channel, JsonSerializer.Serialize(resourceEvent));
    }
}
