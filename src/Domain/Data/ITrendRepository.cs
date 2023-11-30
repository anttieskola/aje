
namespace AJE.Domain.Data;

public interface ITrendRepository
{
    Task<NewsPolarityTrends> GetNewsPolarityTrends(TimePeriod period, DateTimeOffset start, DateTimeOffset end);
}
