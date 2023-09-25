namespace AJE.Application.Entities;
public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Offset { get; }
    public long TotalCount { get; }
    public PaginatedList(IReadOnlyCollection<T> items, int offSet, long totalCount)
    {
        Items = items;
        Offset = offSet;
        TotalCount = totalCount;
    }
}
