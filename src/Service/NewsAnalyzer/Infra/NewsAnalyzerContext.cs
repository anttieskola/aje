namespace AJE.Service.NewsAnalyzer.Infra;

[Table("analysis")]
public class AnalysisRow
{
    [Key]
    [Column("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("polarity")]
    [Column("polarity")]
    public Polarity Polarity { get; set; }

    [Column("polarityversion")]
    public int PolarityVersion { get; set; } = 0;

    [Column("summaryversion")]
    public int SummaryVersion { get; set; } = 0;

    [Column("summary")]
    public string Summary { get; set; } = string.Empty;

    [Column("positivethingsversion")]
    public int PositiveThingsVersion { get; set; } = 0;

    [Column("positivethings")]
    public string PositiveThings { get; set; } = string.Empty;

    [Column("locationsversion")]
    public int LocationsVersion { get; set; } = 0;

    [Column("locations")]
    public string Locations { get; set; } = string.Empty;

    [Column("corporationsversion")]
    public int CorporationsVersion { get; set; } = 0;

    [Column("corporations")]
    public string Corporations { get; set; } = string.Empty;

    [Column("organizationsversion")]
    public int OrganizationsVersion { get; set; } = 0;

    [Column("organizations")]
    public string Organizations { get; set; } = string.Empty;

    [Column("keypeopleversion")]
    public int KeyPeopleVersion { get; set; } = 0;

    [Column("keypeople")]
    public string KeyPeople { get; set; } = string.Empty;
}

public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<AnalysisRow> Analyses { get; set; } = null!;
}
