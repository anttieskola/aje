namespace AJE.Domain.Queries;

public record AiGetSentimentPolarityQuery : IRequest<Polarity>
{
    public const int VERSION = PolarityChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetSentimentPolarityQueryHandler : IRequestHandler<AiGetSentimentPolarityQuery, Polarity>
{
    private readonly PolarityChatML _polarityChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetSentimentPolarityQueryHandler(
        IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<Polarity> Handle(AiGetSentimentPolarityQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _polarityChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _polarityChatML.StopWords,
            NumberOfTokensToPredict = 256,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        var polarity = _polarityChatML.Parse(response.Content);
        return polarity;
    }
}