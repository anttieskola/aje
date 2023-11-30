namespace AJE.Domain.Entities;

public record NewsPolarityTrendItem
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset Published { get; init; }
    public required Polarity Polarity { get; init; }
    public required int PolarityVersion { get; init; }
}

public record NewsPolarityTrendCounts
{
    public int Negative { get; set; }
    public EquatableList<NewsPolarityTrendItem> NegativeItems { get; set; } = EquatableList<NewsPolarityTrendItem>.Empty;
    public int Neutral { get; set; }
    public EquatableList<NewsPolarityTrendItem> NeutralItems { get; set; } = EquatableList<NewsPolarityTrendItem>.Empty;
    public int Positive { get; set; }
    public EquatableList<NewsPolarityTrendItem> PositiveItems { get; set; } = EquatableList<NewsPolarityTrendItem>.Empty;
    public int Unknown { get; set; }
    public EquatableList<NewsPolarityTrendItem> UnknownItems { get; set; } = EquatableList<NewsPolarityTrendItem>.Empty;
}

public record NewsPolarityTrendSegment : NewsPolarityTrendCounts
{
    public required TimePeriod TimePeriod { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
}

public record NewsPolarityTrends
{
    public required EquatableList<NewsPolarityTrendSegment> Segments { get; init; } = EquatableList<NewsPolarityTrendSegment>.Empty;
}
