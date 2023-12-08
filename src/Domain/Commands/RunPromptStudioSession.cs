namespace AJE.Domain.Commands;

public record RunPromptStudioSessionCommand : IRequest<PromptStudioSessionCompletedEvent>
{

}

public class RunPromptStudioSessionHandler : IRequestHandler<RunPromptStudioSessionCommand, PromptStudioSessionCompletedEvent>
{
    public Task<PromptStudioSessionCompletedEvent> Handle(RunPromptStudioSessionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}