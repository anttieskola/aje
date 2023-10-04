using AJE.Domain.Commands;

namespace AJE.Application.Commands;

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, ChatMessageSendEvent>
{
    private IConnectionMultiplexer _connection;

    public SendChatMessageCommandHandler(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }
    public async Task<ChatMessageSendEvent> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var sb = _connection.GetSubscriber();
        await sb.PublishAsync(ChatConstants.CHANNEL, JsonSerializer.Serialize(request));
        return new ChatMessageSendEvent();
    }
}
