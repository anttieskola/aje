namespace AJE.Domain.Events;

public record YleEvent
{
    public required Uri Uri { get; init; }
    public required string Explanation { get; init; }
}
