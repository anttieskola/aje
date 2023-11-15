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
        var options = new AiChatOptions
        {
            // currently empty
            // could contain, model info, system instructions, prompt creator...
        };
        var chat = await _aiChatRepository.AddAsync(options);
        var e = new AiChatStartedEvent
        {
            Id = chat.Id,
            Timestamp = DateTimeOffset.UtcNow,
        };
        await _aiChatEventHandler.SendAsync(e);
        return e;
    }
}