namespace AJE.Domain.Queries;

public record AiGetPositiveThingsQuery : IRequest<string>
{
    public const int VERSION = PositiveThingsChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetPositiveThingsQueryHandler : IRequestHandler<AiGetPositiveThingsQuery, string>
{
    private readonly PositiveThingsChatML _positiveThingsChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetPositiveThingsQueryHandler(IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<string> Handle(AiGetPositiveThingsQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _positiveThingsChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _positiveThingsChatML.StopWords,
            NumberOfTokensToPredict = 8192,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        return response.Content;
    }
}