
namespace AJE.Service.NewsTrends.Infra;

public class NewsTrendsContext : DbContext
{
    public NewsTrendsContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

}
