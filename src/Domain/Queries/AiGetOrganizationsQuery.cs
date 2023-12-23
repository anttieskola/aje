namespace AJE.Domain.Queries;

public record AiGetOrganizationsQuery : IRequest<EquatableList<Organization>>
{
    public const int VERSION = OrganizationsChatML.VERSION;
    public required string Context { get; init; }
}

public class AiGetOrganizationsQueryHandler : IRequestHandler<AiGetOrganizationsQuery, EquatableList<Organization>>
{
    private readonly OrganizationsChatML _organizationsChatML = new();
    private readonly IAiModel _aiModel;

    public AiGetOrganizationsQueryHandler(IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public async Task<EquatableList<Organization>> Handle(AiGetOrganizationsQuery query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query.Context);
        var prompt = _organizationsChatML.Context(query.Context);
        var request = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.1,
            Stop = _organizationsChatML.StopWords,
            NumberOfTokensToPredict = 8192,
        };
        var response = await _aiModel.CompletionAsync(request, cancellationToken);
        return _organizationsChatML.Parse(response.Content);
    }
}