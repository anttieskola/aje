namespace AJE.Domain.Entities;

public record NewsPolarityTrendItem
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset Published { get; init; }
    public required Polarity Polarity { get; init; }
    public required int PolarityVersion { get; init; }
}

public record NewsPolarityTrendSegment
{
    public required TimePeriod TimePeriod { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public required EquatableList<NewsPolarityTrendItem> Items { get; init; } = EquatableList<NewsPolarityTrendItem>.Empty;
    public int PositiveCount
    {
        get
        {
            return Items.Count(x => x.Polarity == Polarity.Positive);
        }
    }
    public int NeutralCount
    {
        get
        {
            return Items.Count(x => x.Polarity == Polarity.Neutral);
        }
    }
    public int NegativeCount
    {
        get
        {
            return Items.Count(x => x.Polarity == Polarity.Negative);
        }
    }
    public int UnknownCount
    {
        get
        {
            return Items.Count(x => x.Polarity == Polarity.Unknown);
        }
    }
    public int TotalCount
    {
        get
        {
            return Items.Count;
        }
    }
}

public record NewsPolarityTrends
{
    public required EquatableList<NewsPolarityTrendSegment> Segments { get; init; } = EquatableList<NewsPolarityTrendSegment>.Empty;
}
