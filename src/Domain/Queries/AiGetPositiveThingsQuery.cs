namespace AJE.Domain.Queries;

public record AiGetPositiveThingsQuery : IRequest<EquatableList<PositiveThing>>
{
    public const int VERSION = PositiveThingsChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetPositiveThingsQueryHandler : IRequestHandler<AiGetPositiveThingsQuery, EquatableList<PositiveThing>>
{
    private readonly PositiveThingsChatML _positiveThingsChatML = new();
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;

    public AiGetPositiveThingsQueryHandler(
        IAiModel aiModel,
        IAiLogger aiLogger)
    {
        _aiModel = aiModel;
        _aiLogger = aiLogger;
    }

    public async Task<EquatableList<PositiveThing>> Handle(AiGetPositiveThingsQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);

        var prompt = _positiveThingsChatML.Context(query.Context);
        int tryCount = 1;
        while (tryCount < 111)
        {
            var settings = CompletionAdjustor.GetSettings(tryCount);
            var request = new CompletionRequest
            {
                Prompt = prompt,
                Temperature = settings.Temperature,
                TopK = settings.TopK,
                Stop = _positiveThingsChatML.StopWords,
                NumberOfTokensToPredict = 4096,
            };
            var response = await _aiModel.CompletionAsync(request, cancellationToken);
            try
            {
                return _positiveThingsChatML.Parse(response.Content);
            }
            catch (AiParseException)
            {
                tryCount++;
            }
            _aiLogger.Log($"Failed to parse PositiveThings TryCount:{tryCount-1}");
        }
        throw new AiException("Failed to get PositiveThings");
    }
}