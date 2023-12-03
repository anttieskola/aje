namespace AJE.Domain.Queries;

public record GetArticleSentimentPolarityTrendsQuery : IRequest<NewsPolarityTrendSegment[]>
{
    public required ArticleCategory ArticleCategory { get; init; }
    public required TimePeriod TimePeriod { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
}

public class GetArticleSentimentPolarityTrendsQueryHandler : IRequestHandler<GetArticleSentimentPolarityTrendsQuery, NewsPolarityTrendSegment[]>
{
    private readonly ITrendRepository _trendRepository;

    public GetArticleSentimentPolarityTrendsQueryHandler(ITrendRepository trendRepository)
    {
        _trendRepository = trendRepository;
    }

    public async Task<NewsPolarityTrendSegment[]> Handle(GetArticleSentimentPolarityTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _trendRepository.GetArticleSentimentPolarityTrendsAsync(request, cancellationToken);
    }
}
