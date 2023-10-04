namespace AJE.Service.News.Yle;

public class YleFeed
{
    public required string Name { get; set; }
    public required Uri Url { get; set; }
}

public class YleConfiguration
{
    public int RefreshDelayInSeconds { get; set; }
    public required string DumpFolder { get; set; }
    public required List<YleFeed> Feeds { get; set; }
}
