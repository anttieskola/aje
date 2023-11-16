namespace AJE.Domain.Events;
public record ChatMessageSendEvent
{
    public required string User { get; init; }
    public required string Message { get; init; }
}