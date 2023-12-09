namespace AJE.Application.Commands;

public class SendChatMessageCommandHandler : IRequestHandler<ChatMessageSendCommand, ChatMessageSendEvent>
{
    private readonly IConnectionMultiplexer _connection;

    public SendChatMessageCommandHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }
    public async Task<ChatMessageSendEvent> Handle(ChatMessageSendCommand request, CancellationToken cancellationToken)
    {
        var sb = _connection.GetSubscriber();
        var msg = JsonSerializer.Serialize(request);
        var channel = new RedisChannel(ChatConstants.CHANNEL, RedisChannel.PatternMode.Auto);
        await sb.PublishAsync(channel, msg);
        return new ChatMessageSendEvent
        {
            User = request.User,
            Message = request.Message
        };
    }
}
