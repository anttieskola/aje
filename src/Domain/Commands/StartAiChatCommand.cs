namespace AJE.Domain.Commands;

// could contain, model info, system instructions, prompt creator, temperature...
public record StartAiChatCommand : IRequest<AiChatEvent>
{
    public required Guid Id { get; init; }
}

public class StartAiChatCommandHandler : IRequestHandler<StartAiChatCommand, AiChatEvent>
{
    private readonly IAiChatRepository _aiChatRepository;
    private readonly IAiChatEventHandler _aiChatEventHandler;

    public StartAiChatCommandHandler(
        IAiChatRepository aiChatRepository,
        IAiChatEventHandler aiChatEventHandler)
    {
        _aiChatRepository = aiChatRepository;
        _aiChatEventHandler = aiChatEventHandler;
    }

    public async Task<AiChatEvent> Handle(StartAiChatCommand command, CancellationToken cancellationToken)
    {
        var options = new AiChatOptions
        {
            ChatId = command.Id,
        };
        var chat = await _aiChatRepository.AddAsync(options);
        var e = new AiChatStartedEvent
        {
            ChatId = chat.ChatId,
            StartTimestamp = DateTimeOffset.UtcNow,
        };
        await _aiChatEventHandler.SendAsync(e);
        return e;
    }
}