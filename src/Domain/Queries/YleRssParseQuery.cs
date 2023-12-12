namespace AJE.Domain.Queries;

public record YleRssParseQuery : IRequest<string[]>
{
    public required XDocument Rss { get; init; }
}

public class YleRssParseQueryHandler : IRequestHandler<YleRssParseQuery, string[]>
{
    public Task<string[]> Handle(YleRssParseQuery request, CancellationToken cancellationToken)
    {
        var list = new List<string>();
        var items = from item in request.Rss.Descendants("item")
                    select item;

        foreach (var item in items)
        {
            var articleLink = item.Element("link");
            if (articleLink != null)
            {
                list.Add(articleLink.Value.Replace("?origin=rss", string.Empty));
            }
        }
        return Task.FromResult(list.ToArray());
    }
}
