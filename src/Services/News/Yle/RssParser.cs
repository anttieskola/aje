namespace AJE.Service.News.Yle;

public static class RssParser
{
    public static IEnumerable<string> Parse(string rss)
    {
        var doc = XDocument.Parse(rss);
        var list = new List<string>();
        var items = from item in doc.Descendants("item")
                   select item;

        foreach (var item in items)
        {
            var articleLink = item.Element("link");
            if (articleLink != null)
            {
                list.Add(articleLink.Value);
            }
        }
        return list;
    }
}
