namespace AJE.Domain.Queries;

public record AiGetSummaryQuery : IRequest<string>
{
    public const int VERSION = SummaryChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetSummaryQueryHandler : IRequestHandler<AiGetSummaryQuery, string>
{
    private readonly SummaryChatML _summaryChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetSummaryQueryHandler(IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<string> Handle(AiGetSummaryQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _summaryChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _summaryChatML.StopWords,
            NumberOfTokensToPredict = 8192,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        return response.Content;
    }
}