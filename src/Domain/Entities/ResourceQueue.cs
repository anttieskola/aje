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

    public bool IsFree() => _current == Guid.Empty;
    public Guid Current() => _current;
    public bool IsQueue() => _requests.Count > 1;
    public int QueueCount() => _current == Guid.Empty ? _requests.Count : _requests.Count - 1;
    public int TotalCount() => _count;
}
