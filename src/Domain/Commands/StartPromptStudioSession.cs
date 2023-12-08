
namespace AJE.Domain.Commands;

public record StartPromptStudioSessionCommand : IRequest<PromptStudioSessionStartedEvent>
{

}

public class StartPromptStudioSessionHandler : IRequestHandler<StartPromptStudioSessionCommand, PromptStudioSessionStartedEvent>
{
    public Task<PromptStudioSessionStartedEvent> Handle(StartPromptStudioSessionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}