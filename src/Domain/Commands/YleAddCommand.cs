namespace AJE.Domain.Commands;

public record YleAddCommand : IRequest<YleEvent>
{
    public required Uri Uri { get; init; }
    public required string Html { get; init; }
}

public class YleAddCommandHandler : IRequestHandler<YleAddCommand, YleEvent>
{
    private readonly IYleRepository _repository;
    private readonly IYleEventHandler _eventHandler;

    public YleAddCommandHandler(
        IYleRepository repository,
        IYleEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
    }

    public async Task<YleEvent> Handle(YleAddCommand command, CancellationToken cancellationToken)
    {
        await _repository.StoreAsync(command.Uri, command.Html, cancellationToken);
        var yleEvent = new YleEvent
        {
            Uri = command.Uri,
            Explanation = "Added"
        };
        await _eventHandler.SendAsync(yleEvent);
        return yleEvent;
    }
}
