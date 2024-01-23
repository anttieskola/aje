namespace AJE.Infra.Database;

public class AiStoryRow
{
    public Guid StoryId { get; set; }
}

public class AiSettingsRow
{
    public double Temperature { get; set; }
    public int TopK { get; set; }
    public int NumberOfTokensToPredict { get; set; }
}

public class AiStoryContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{

    public DbSet<AiStoryRow> AiStories { get; set; }
}
