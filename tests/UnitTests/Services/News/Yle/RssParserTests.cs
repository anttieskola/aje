using AJE.Service.News.Yle;

namespace AJE.UnitTests.Services.News.Yle;
public class RssParserTests
{
    private string _rssEmpty = @"
<rss version=""2.0"">
</rss>
";
    private string _rssTwo = @"
<rss version=""2.0"">
    <channel>
        <language>en</language>
        <title>Yle News | Tuoreimmat uutiset</title>
        <description>Yle News | Tuoreimmat uutiset</description>
        <item>
            <title>Orpo: Street violence partly due to unsuccessful integration of immigrants</title>
            <link>https://yle.fi/a/74-20052790?origin=rss</link>
            <description>Prime Minister Petteri Orpo (NCP) said on Sunday that his government plans tougher penalties and other means to ensure that gang-related crime in Finland does not explode as it has in Sweden.</description>
            <pubDate>Sun, 01 Oct 2023 18:25:13 +0300</pubDate>
            <category>politiikka</category>
            <category>Jengiväkivalta</category>
            <category>rikokset</category>
            <category>maahanmuutto</category>
            <guid isPermaLink=""false"">https://yle.fi/a/74-20052790</guid>
            <enclosure url=""https://images.cdn.yle.fi/image/upload//w_205,h_115,q_70/39-11776736513f4d4841b6.jpg"" type=""image/jpeg"" length=""0""/>
        </item>
        <item>
            <title>Helsinki Airport security staff to walk off the job on Thursday</title>
            <link>https://yle.fi/a/74-20052777?origin=rss</link>
            <description>The Trade Union for the Public and Welfare Sectors (JHL), the Trade Union Pro and the Industrial Union will all organise walkouts next Thursday.</description>
            <pubDate>Sun, 01 Oct 2023 13:44:11 +0300</pubDate>
            <category>työmarkkinat</category>
            <category>talous</category>
            <category>Helsinki-Vantaan lentoasema</category>
            <category>Julkisten ja hyvinvointialojen liitto</category>
            <guid isPermaLink=""false"">https://yle.fi/a/74-20052777</guid>
            <enclosure url=""https://images.cdn.yle.fi/image/upload//w_205,h_115,q_70/39-11217546475b239c7bed.jpg"" type=""image/jpeg"" length=""0""/>
        </item>
    </channel>
</rss>
";

    [Fact]
    public void Parse()
    {
        var links = RssParser.Parse(_rssEmpty);
        Assert.Empty(links);

        links = RssParser.Parse(_rssTwo);
        Assert.Equal(2, links.Count());
        Assert.Equal("https://yle.fi/a/74-20052790", links.ElementAt(0));
        Assert.Equal("https://yle.fi/a/74-20052777", links.ElementAt(1));
    }
}
