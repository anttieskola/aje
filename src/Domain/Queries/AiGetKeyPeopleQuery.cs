namespace AJE.Domain.Queries;

public record AiGetKeyPeopleQuery : IRequest<EquatableList<KeyPerson>>
{
    public const int VERSION = KeyPeopleChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetKeyPeopleQueryHandler : IRequestHandler<AiGetKeyPeopleQuery, EquatableList<KeyPerson>>
{
    private readonly KeyPeopleChatML _keyPersonsChatML = new();
    private readonly IAiModel _aiModel;
    private readonly IAiLogger _aiLogger;
    public AiGetKeyPeopleQueryHandler(
        IAiModel aiModel,
        IAiLogger aiLogger)
    {
        _aiModel = aiModel;
        _aiLogger = aiLogger;
    }

    public async Task<EquatableList<KeyPerson>> Handle(AiGetKeyPeopleQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);

        var prompt = _keyPersonsChatML.Context(query.Context);
        int tryCount = 1;
        while (tryCount < 111)
        {
            var settings = CompletionAdjustor.GetSettings(tryCount);
            var request = new CompletionRequest
            {
                Prompt = prompt,
                Temperature = settings.Temperature,
                TopK = settings.TopK,
                Stop = _keyPersonsChatML.StopWords,
                NumberOfTokensToPredict = 4096,
            };
            var response = await _aiModel.CompletionAsync(request, cancellationToken);
            try
            {
                return _keyPersonsChatML.Parse(response.Content);
            }
            catch (AiParseException)
            {
                tryCount++;
            }
            _aiLogger.Log($"Failed to parse KeyPeople TryCount:{tryCount - 1}");
        }
        throw new AiException("Failed to get KeyPersons");
    }
}