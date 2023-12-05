namespace AJE.Domain.Data;

public interface ITrendRepository
{
    Task<NewsPolarityTrendSegment[]> GetArticleSentimentPolarityTrendsAsync(GetArticleSentimentPolarityTrendsQuery query, CancellationToken cancellationToken);
}
