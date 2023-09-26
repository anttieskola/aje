namespace AJE.Application.Commands;

public record SendChatMessageCommand : IRequest<ChatMessageSendEvent>
{
    public required string ChatChannel { get; init; }
    public required string User { get; init; }
    public required string Message { get; init; }
}

public class SendChatMessage : IRequestHandler<SendChatMessageCommand, ChatMessageSendEvent>
{
    private IConnectionMultiplexer _connection;

    public SendChatMessage(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }
    public async Task<ChatMessageSendEvent> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var sb = _connection.GetSubscriber();
        await sb.PublishAsync(AJEConstants.CHANNEL_CHAT, JsonSerializer.Serialize(request));
        return new ChatMessageSendEvent();
    }
}
