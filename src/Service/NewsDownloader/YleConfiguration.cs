namespace AJE.Service.NewsDownloader;

public class YleFeed
{
    public required string Name { get; set; }
    public required Uri Url { get; set; }
}

public class YleConfiguration
{
    public required bool PublishOnlyEnglish { get; set; }
    public required int RefreshDelayInSeconds { get; set; }
    public required List<YleFeed> Feeds { get; set; }
}
