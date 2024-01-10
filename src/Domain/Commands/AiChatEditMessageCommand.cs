
namespace AJE.Domain.Commands;

public class AiChatEditMessageCommand : IRequest<AiChatEvent>
{
    public bool IsTest { get; init; } = false;
    public required Guid ChatId { get; init; }
    public required Guid InteractionId { get; init; }
}

public class AiChatEditMessageCommandHandler : IRequestHandler<AiChatEditMessageCommand, AiChatEvent>
{
    public AiChatEditMessageCommandHandler()
    {

    }

    public Task<AiChatEvent> Handle(AiChatEditMessageCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
