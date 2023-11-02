namespace AJE.Domain.Commands;

/// <summary>
/// Reserved for settings like
/// - used model (nothing is stopping us from chatting with multiple models...)
///    - maybe we are missing HW though
/// - system instructions
/// - prompt creator
/// ...
/// </summary>
public record StartAiChatCommand : IRequest<AiChatEvent>
{
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
        var aiChat = new AiChat
        {
            Id = Guid.NewGuid(),
        };
        await _aiChatRepository.AddAsync(aiChat);
        var e = new AiChatStartedEvent
        {
            Id = aiChat.Id,
            Timestamp = DateTimeOffset.UtcNow,
        };
        await _aiChatEventHandler.SendAsync(e);
        return e;
    }
}