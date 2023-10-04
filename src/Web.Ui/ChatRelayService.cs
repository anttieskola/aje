namespace AJE.Web.Ui
{
    /// <summary>
    /// This service simply relays chat messages from redis to local clients
    /// So current message name is just chat, to send message there
    /// PUBLISH chat "{\"user\":\"Redis\",\"message\":\"Hello clients\"}"
    /// Then it gets routed here to signal-r messaging channels
    /// </summary>
    public class ChatRelayService : BackgroundService
    {
        private readonly ILogger<ChatRelayService> _logger;
        private readonly IHubContext<LocalChatHub, IChatClient> _hubContext;
        private readonly IConnectionMultiplexer _connection;

        public ChatRelayService(
            ILogger<ChatRelayService> logger,
            IHubContext<LocalChatHub, IChatClient> hubContext,
            IConnectionMultiplexer connection)
        {
            _logger = logger;
            _hubContext = hubContext;
            _connection = connection;
        }


        private bool _subscribed = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_subscribed)
                {
                    await Subscribe();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                await _hubContext.Clients.All.ReceiveMessage("System", "Hello clients");
            }
        }

        private async Task Subscribe()
        {
            var s = _connection.GetSubscriber();
            await s.SubscribeAsync(ChatConstants.CHANNEL, HandleMessage);
        }

        private void HandleMessage(RedisChannel channel, RedisValue value)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<ChatMessage>(value.ToString());
                if (msg != null)
                {
                    _hubContext.Clients.All.ReceiveMessage(msg!.UserName, msg!.Message);
                }
            } catch (Exception e)
            {
                _logger.LogError(e, "Failed to deserialize message");
            }
        }
    }
}
