namespace AJE.Domain.Queries;

public record YleGetQuery : IRequest<string>
{
    public required Uri Uri { get; init; }
}

public class YleGetQueryHandler : IRequestHandler<YleGetQuery, string>
{
    private readonly IYleRepository _repository;

    public YleGetQueryHandler(IYleRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(YleGetQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetHtmlAsync(query.Uri);
    }
}