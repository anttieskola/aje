namespace AJE.Domain.Commands;

public record YleUpdateCommand : IRequest<YleEvent>
{
    public required Uri Uri { get; init; }
    public required string Html { get; init; }
}

public class YleUpdateCommandHandler : IRequestHandler<YleUpdateCommand, YleEvent>
{
    private readonly IYleRepository _repository;
    private readonly IYleEventHandler _eventHandler;

    public YleUpdateCommandHandler(
        IYleRepository repository,
        IYleEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
    }

    public async Task<YleEvent> Handle(YleUpdateCommand command, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(command.Uri, command.Html, cancellationToken);
        var yleEvent = new YleEvent
        {
            Uri = command.Uri,
            Explanation = "Updated"
        };
        await _eventHandler.SendAsync(yleEvent);
        return yleEvent;
    }
}