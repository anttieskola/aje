namespace AJE.Application.Commands;

public record SendChatMessageCommand : IRequest<ChatMessageSendEvent>
{
    public string User { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
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
        var cm = new ChatMessage
        {
            User = request.User,
            Message = request.Message
        };
        var msg = JsonSerializer.Serialize(cm);
        await sb.PublishAsync("chat", msg);
        return new ChatMessageSendEvent();
    }
}
