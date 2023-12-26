namespace AJE.Domain.Entities;

/// <summary>
/// Collection of silly AI analysis ideas to score on
/// </summary>
public record Analysis
{
    [JsonPropertyName("summaryVersion")]
    public int SummaryVersion { get; set; } = 0;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("positiveThingsVersion")]
    public int PositiveThingsVersion { get; set; } = 0;

    [JsonPropertyName("positiveThings")]
    public EquatableList<PositiveThing> PositiveThings { get; set; } = [];

    [JsonPropertyName("locationsVersion")]
    public int LocationsVersion { get; set; } = 0;

    [JsonPropertyName("locations")]
    public EquatableList<Location> Locations { get; set; } = [];

    [JsonPropertyName("corporationsVersion")]
    public int CorporationsVersion { get; set; } = 0;

    [JsonPropertyName("corporations")]
    public EquatableList<Corporation> Corporations { get; set; } = [];

    [JsonPropertyName("organizationsVersion")]
    public int OrganizationsVersion { get; set; } = 0;

    [JsonPropertyName("organizations")]
    public EquatableList<Organization> Organizations { get; set; } = [];

    [JsonPropertyName("keyPeopleVersion")]
    public int KeyPeopleVersion { get; set; } = 0;

    [JsonPropertyName("keyPeople")]
    public EquatableList<KeyPerson> KeyPeople { get; set; } = [];
}