namespace AJE.Domain.Queries;

public record ArticleGetSentimentPolarityTrendsQuery : IRequest<NewsPolarityTrendSegment[]>
{
    public required ArticleCategory ArticleCategory { get; init; }
    public required TimePeriod TimePeriod { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
}

public class ArticleGetSentimentPolarityTrendsQueryHandler : IRequestHandler<ArticleGetSentimentPolarityTrendsQuery, NewsPolarityTrendSegment[]>
{
    private readonly ITrendRepository _trendRepository;

    public ArticleGetSentimentPolarityTrendsQueryHandler(ITrendRepository trendRepository)
    {
        _trendRepository = trendRepository;
    }

    public async Task<NewsPolarityTrendSegment[]> Handle(ArticleGetSentimentPolarityTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _trendRepository.GetArticleSentimentPolarityTrendsAsync(request, cancellationToken);
    }
}
