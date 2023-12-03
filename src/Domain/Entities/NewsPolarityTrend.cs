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
            return Items.Where(x => x.Polarity == Polarity.Positive).Count();
        }
    }
    public int NeutralCount
    {
        get
        {
            return Items.Where(x => x.Polarity == Polarity.Neutral).Count();
        }
    }
    public int NegativeCount
    {
        get
        {
            return Items.Where(x => x.Polarity == Polarity.Negative).Count();
        }
    }
    public int UnknownCount
    {
        get
        {
            return Items.Where(x => x.Polarity == Polarity.Unknown).Count();
        }
    }
}

public record NewsPolarityTrends
{
    public required EquatableList<NewsPolarityTrendSegment> Segments { get; init; } = EquatableList<NewsPolarityTrendSegment>.Empty;
}
