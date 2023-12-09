namespace AJE.Domain.Data;

public interface ITrendRepository
{
    Task<NewsPolarityTrendSegment[]> GetArticleSentimentPolarityTrendsAsync(ArticleGetSentimentPolarityTrendsQuery query, CancellationToken cancellationToken);
}
