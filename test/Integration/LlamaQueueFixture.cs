using AJE.Service.Manager;
using Microsoft.Extensions.Logging;

namespace AJE.Test.Integration;

public class LlamaQueueFixture : IDisposable
{
    private readonly IConnectionMultiplexer _connection;
    private readonly LlamaQueueManager _llamaQueueManager;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public LlamaQueueFixture()
    {
        _connection = ConnectionMultiplexer.Connect(TestConstants.RedisAddress);
        var listName = TestConstants.LlamaConfiguration.Servers[0].ResourceName;
        var length = _connection.GetDatabase().ListLength(listName);
        for (int i = 0; i < length; i++)
        {
            _connection.GetDatabase().ListRightPop(listName);
        }

        _llamaQueueManager = new LlamaQueueManager(
            new Mock<ILogger<LlamaQueueManager>>().Object,
            _connection,
            TestConstants.LlamaConfiguration,
            true);

        Task.Factory.StartNew(() =>
        {
            _llamaQueueManager.StartAsync(_cancellationTokenSource.Token);
        });
    }


    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _connection.Dispose();
                _llamaQueueManager.Dispose();
                _cancellationTokenSource.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
