namespace AJE.Domain.Queries;

public record YleListQuery : IRequest<Uri[]>
{
}

public class YleHtmlListQueryHandler : IRequestHandler<YleListQuery, Uri[]>
{
    private readonly IYleRepository _repository;

    public YleHtmlListQueryHandler(IYleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Uri[]> Handle(YleListQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetUriList();
    }
}