namespace AJE.Domain.Queries;

public record YleRssParseQuery : IRequest<Uri[]>
{
    public required XDocument Rss { get; init; }
}

public class YleRssParseQueryHandler : IRequestHandler<YleRssParseQuery, Uri[]>
{
    public Task<Uri[]> Handle(YleRssParseQuery query, CancellationToken cancellationToken)
    {
        var list = new List<Uri>();
        var items = from item in query.Rss.Descendants("item")
                    select item;

        foreach (var item in items)
        {
            var articleLink = item.Element("link");
            if (articleLink != null)
            {
                list.Add(new Uri(articleLink.Value.Replace("?origin=rss", string.Empty)));
            }
        }
        return Task.FromResult(list.ToArray());
    }
}
