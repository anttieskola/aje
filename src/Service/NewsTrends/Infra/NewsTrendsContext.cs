
namespace AJE.Service.NewsTrends.Infra;

public class NewsTrendsContext : DbContext
{
    private readonly ISender _sender;

    public NewsTrendsContext(DbContextOptions dbContextOptions, ISender sender)
        : base(dbContextOptions)
    {
        _sender = sender;
    }

    public DbSet<NewsPolarityTrendSegment> NewsPolarityTrendSegments { get; set; } = null!;
}

public class NewsTrendCache
{
    private readonly ILogger<NewsTrendsContext> _logger;
    private readonly ISender _sender;

    public NewsTrendCache(
        ILogger<NewsTrendsContext> logger,
        ISender sender
    )
    {
        _logger = logger;
        _sender = sender;

    }

    public static ConcurrentDictionary<Guid, NewsPolarityTrendSegment> NewsPolarityTrendSegments { get; set; } = new();
}