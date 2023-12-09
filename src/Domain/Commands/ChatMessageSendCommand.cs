namespace AJE.Domain.Commands;

public record ChatMessageSendCommand : IRequest<ChatMessageSendEvent>
{
    public required string ChatChannel { get; init; }
    public required string User { get; init; }
    public required string Message { get; init; }
}
