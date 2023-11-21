namespace AJE.Util.Messenger;

/// <summary>
/// This is a test program to simulate event handling.
/// It will run multiple threads
/// - One thread will run event handler and subscribe to AiChatEvent's
///     - Event handler when someone subscribed, will run own thread that is subscribed to redis and distributes events
/// - Main thread will run also event handler but only to send events
/// Note that eventhandler is singleton so both threads will use same instance
/// </summary>
static class Program
{
    public static async Task Main(string[] args)
    {
        // app initialization
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(logging =>
        {
            logging.AddSimpleConsole(option =>
            {
                option.SingleLine = true;
            });
        });
        serviceCollection.AddDomain();
        serviceCollection.AddRedis(config);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        await serviceProvider.InitializeRedis();

        // startup
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var eventHandlerThread = new Thread(() =>
        {
            var subscriberId = Guid.NewGuid();
            var eventHandler = serviceProvider.GetRequiredService<IAiChatEventHandler>();
            eventHandler.Subscribe(subscriberId, async (aiChatEvent) =>
            {
                var ticks = DateTime.UtcNow.Ticks;
                if (cancellationToken.IsCancellationRequested)
                {
                    eventHandler.Unsubscribe(subscriberId);
                    return;
                }
                Console.WriteLine($"Received: {ticks}, with StartTimestamp:{aiChatEvent.StartTimestamp}");
                await Task.CompletedTask;
            });
        })
        {
            IsBackground = true,
            Name = "AiChatEventHandler",
        };
        eventHandlerThread.Start();

        // this thread will send events
        Console.WriteLine("Press any key to exit");
        var eventHandler = serviceProvider.GetRequiredService<IAiChatEventHandler>();
        while (!Console.KeyAvailable)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var startTimestamp = new DateTimeOffset(new DateTime(ticks, DateTimeKind.Utc));
            await eventHandler.SendAsync(new AiChatStartedEvent
            {
                IsTest = true,
                ChatId = Guid.NewGuid(),
                StartTimestamp = startTimestamp,
            });
            Console.WriteLine($"Sent    : {ticks}, with StartTimestamp:{startTimestamp}");
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        // exit
        cancellationTokenSource.Cancel();
        Console.WriteLine("All cleaned up exiting");
    }
}
