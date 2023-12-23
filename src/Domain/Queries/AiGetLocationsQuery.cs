namespace AJE.Domain.Queries;

public record AiGetLocationsQuery : IRequest<EquatableList<Location>>
{
    public const int VERSION = LocationsChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetLocationsQueryHandler : IRequestHandler<AiGetLocationsQuery, EquatableList<Location>>
{
    private readonly LocationsChatML _locationsChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetLocationsQueryHandler(IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<EquatableList<Location>> Handle(AiGetLocationsQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _locationsChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _locationsChatML.StopWords,
            NumberOfTokensToPredict = 8192,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        return _locationsChatML.Parse(response.Content);
    }
}
