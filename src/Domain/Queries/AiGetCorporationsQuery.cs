namespace AJE.Domain.Queries;

public record AiGetCorporationsQuery : IRequest<EquatableList<Corporation>>
{
    public const int VERSION = CorporationsChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetCorporationsQueryHandler : IRequestHandler<AiGetCorporationsQuery, EquatableList<Corporation>>
{
    private readonly CorporationsChatML _corporationsChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetCorporationsQueryHandler(IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<EquatableList<Corporation>> Handle(AiGetCorporationsQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _corporationsChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _corporationsChatML.StopWords,
            NumberOfTokensToPredict = 8192,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        return _corporationsChatML.Parse(response.Content);
    }
}