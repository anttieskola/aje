namespace AJE.Domain.Queries;

public record GetNewsPolarityTrendsQuery : IRequest<NewsPolarityTrends>
{
    public TimePeriod Period { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}

public class GetNewsPolarityTrendsQueryHandler : IRequestHandler<GetNewsPolarityTrendsQuery, NewsPolarityTrends>
{
    private readonly ITrendRepository _trendRepository;

    public GetNewsPolarityTrendsQueryHandler(ITrendRepository trendRepository)
    {
        _trendRepository = trendRepository;
    }

    public async Task<NewsPolarityTrends> Handle(GetNewsPolarityTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _trendRepository.GetNewsPolarityTrends(request.Period, request.Start, request.End);
    }
}
