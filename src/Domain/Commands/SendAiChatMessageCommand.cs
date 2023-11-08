namespace AJE.Domain.Commands;

public record SendAiChatMessageCommand : IRequest<AiChatEvent>
{
    public required Guid ChatId { get; init; }

    public required string Message { get; init; }

    public required Stream Output { get; init; }
}

public class SendAiChatMessageCommandHandler : IRequestHandler<SendAiChatMessageCommand, AiChatEvent>
{
    private readonly IAiChatRepository _aiChatRepository;
    private readonly IAiModel _aiModel;

    public SendAiChatMessageCommandHandler(
        IAiChatRepository aiChatRepository,
        IAiModel aiModel)
    {
        _aiChatRepository = aiChatRepository;
        _aiModel = aiModel;
    }

    public async Task<AiChatEvent> Handle(SendAiChatMessageCommand command, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        // create full context using chat history data from Redis thrue repository
        // create prompt using AnttiChatML
        throw new NotImplementedException();
    }
}
