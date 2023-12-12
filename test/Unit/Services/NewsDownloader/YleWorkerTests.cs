using AJE.Service.NewsDownloader;

namespace AJE.Test.Unit.Services.News.Yle;

public class YleWorkerTests
{
    [Fact]
    public void CreateRSSFileName()
    {
        var uri = new Uri("https://feeds.yle.fi/uutiset/v1/recent.rss?publisherIds=YLE_NEWS");
        var fileName = YleWorker.CreateRSSFileName(uri);
        Assert.Equal("httpsfeedsylefiuutisetv1recentrsspublisherIdsYLENEWS.xml", fileName);
    }

    [Fact]
    public void CreateHTMLFileName()
    {
        var url = "https://yle.fi/a/74-20052790";
        var fileName = YleWorker.CreateHTMLFileName(url);
        Assert.Equal("74-20052790.html", fileName);
    }
}
