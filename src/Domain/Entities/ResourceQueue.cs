namespace AJE.Domain.Entities;

/// <summary>
/// Queue manager that works directly witht he resource events
/// Queue is stored in memory only so will be lost on restart
/// </summary>
/// <param name="resourceName"></param>
public class ResourceQueue(
    string resourceName)
{
    private readonly string _resourceName = resourceName;
    private int _count = 0;
    private readonly ConcurrentDictionary<Guid, DateTimeOffset> _requests = new();
    private Guid _current = Guid.Empty;

    public void Request(ResourceRequestEvent requestEvent)
    {
        if (!_requests.TryAdd(requestEvent.RequestId, DateTimeOffset.UtcNow))
        {
            throw new PlatformException("Failed to add request");
        }
        _count++;
    }

    public void Release(ResourceReleasedEvent releaseEvent)
    {
        if (!_requests.TryRemove(releaseEvent.RequestId, out _))
        {
            throw new PlatformException("Failed to remove request");
        }
        else
        {
            if (releaseEvent.RequestId == _current)
                _current = Guid.Empty;
        }
    }

    public ResourceGrantedEvent? GetNext()
    {
        var next = _requests.OrderBy(r => r.Value).FirstOrDefault();
        if (next.Key != Guid.Empty)
        {
            _current = next.Key;
            return new ResourceGrantedEvent
            {
                ResourceName = _resourceName,
                RequestId = next.Key
            };
        }
        return null;
    }

    /// <summary>
    /// If current active request has been running more than given minutes
    /// we remove it, make resource free and return the event for releasing
    /// Purpose is to remove crashed requests from the queue one by one
    /// Bad thing is that if crashed component has multiple requests...
    /// it takes time to get them all removed
    /// </summary>
    /// <param name="intMinutesOlderThan"></param>
    /// <returns></returns>
    /// <exception cref="PlatformException"></exception>
    public ResourceReleasedEvent? Cleanup(int intMinutesOlderThan)
    {
        if (_current != Guid.Empty)
        {
            var r = _requests.Single(r => r.Key == _current);
            if (r.Value <= DateTimeOffset.UtcNow.AddMinutes(-1 * intMinutesOlderThan))
            {
                if (!_requests.TryRemove(r.Key, out _))
                {
                    throw new PlatformException("Failed to remove request");
                }
                else
                {
                    _current = Guid.Empty;
                    return new ResourceReleasedEvent
                    {
                        ResourceName = _resourceName,
                        RequestId = r.Key
                    };
                }
            }
        }
        return null;
    }

    public bool IsFree() => _current == Guid.Empty;
    public Guid Current() => _current;
    public bool IsQueue() => _requests.Count > 1;
    public int QueueCount() => _current == Guid.Empty ? _requests.Count : _requests.Count - 1;
    public int TotalCount() => _count;
}
