namespace AJE.Domain.Queries;

public record YleExistsQuery : IRequest<bool>
{
    public required Uri Uri { get; init; }
}

public class YleExistsQueryHandler : IRequestHandler<YleExistsQuery, bool>
{
    private readonly IYleRepository _repository;

    public YleExistsQueryHandler(IYleRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(YleExistsQuery query, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(query.Uri, cancellationToken);
    }
}