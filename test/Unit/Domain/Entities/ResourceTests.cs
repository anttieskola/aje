using AJE.Domain.Entities;
using AJE.Domain.Events;

namespace AJE.Test.Unit.Domain.Entities;

public class ResourceTests
{
    [Fact]
    public async Task LifeCycle()
    {
        // request id's
        var r1Id = Guid.NewGuid();
        var r2Id = Guid.NewGuid();
        var r3Id = Guid.NewGuid();
        var r4Id = Guid.NewGuid();

        // arrange
        var management = new ResourceQueue("test");

        // act, requiest,so no queuu, is free, get item
        management.Request(new ResourceRequestEvent { ResourceName = "test", RequestId = r1Id });
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        Assert.False(management.IsQueue());
        Assert.True(management.IsFree());
        {
            var grantEvent = management.GetNext();
            Assert.NotNull(grantEvent);
            Assert.Equal(r1Id, grantEvent.RequestId);
        }
        Assert.False(management.IsFree());
        Assert.Equal(r1Id, management.Current());

        // r1 is active, make more requests 2,3,4
        management.Request(new ResourceRequestEvent { ResourceName = "test", RequestId = r2Id });
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        management.Request(new ResourceRequestEvent { ResourceName = "test", RequestId = r3Id });
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        management.Request(new ResourceRequestEvent { ResourceName = "test", RequestId = r4Id });
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        Assert.Equal(4, management.TotalCount());
        Assert.Equal(3, management.QueueCount());
        Assert.True(management.IsQueue());
        Assert.False(management.IsFree());

        // release 4th (give up waiting)
        management.Release(new ResourceReleasedEvent { ResourceName = "test", RequestId = r4Id });
        Assert.Equal(4, management.TotalCount());
        Assert.Equal(2, management.QueueCount());
        Assert.True(management.IsQueue());
        Assert.False(management.IsFree());

        // release 1st, take 2nd
        management.Release(new ResourceReleasedEvent { ResourceName = "test", RequestId = r1Id });
        {
            var grantEvent = management.GetNext();
            Assert.NotNull(grantEvent);
            Assert.Equal(r2Id, grantEvent.RequestId);
        }
        Assert.Equal(r2Id, management.Current());

        // release 2nd, take 3rd
        management.Release(new ResourceReleasedEvent { ResourceName = "test", RequestId = r2Id });
        Assert.False(management.IsQueue());
        {
            var grantEvent = management.GetNext();
            Assert.NotNull(grantEvent);
            Assert.Equal(r3Id, grantEvent.RequestId);
        }
        Assert.Equal(r3Id, management.Current());

        // release 3rd, nothing left
        management.Release(new ResourceReleasedEvent { ResourceName = "test", RequestId = r3Id });
        Assert.False(management.IsQueue());
        Assert.Null(management.GetNext());
        Assert.Equal(4, management.TotalCount());
    }
}
