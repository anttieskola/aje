namespace AJE.Domain.Entities;

public record AiChatHistory
{
    public required Guid Id { get; init; }
    public EquatableList<AiChatMessage> Messages { get; init; } = new();
}

public enum AiChatRole
{
    User = 1,
    Bot = 2
}

public record AiChatMessage
{
    public AiChatRole Role { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public required string Message { get; init; }
}
