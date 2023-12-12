using System.Xml.Linq;
using AJE.Domain.Queries;

namespace AJE.Test.Unit.Domain.Queries;

public class YleParseRssQueryHandlerTests
{
    private readonly string _rssEmpty = @"
<rss version=""2.0"">
</rss>
";
    [Fact]
    public async Task Empty()
    {
        // arrange
        var rss = XDocument.Parse(_rssEmpty);
        var handler = new YleRssParseQueryHandler();
        // act
        var result = await handler.Handle(new YleRssParseQuery { Rss = rss }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private readonly string _rssSome = @"
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
        <item>
          <title>Vastaavalta päätoimittajalta: Uutismedioillakin on vielä tasa-arvotyötä tehtävänä</title>
          <link>https://yle.fi/a/3-12127507?origin=rss</link>
          <description>Tytöt kaikkialla maailmassa kohtaavat itseään koskevaa väärää tietoa ja olettamuksia. Lapsilla ja nuorilla sukupuoleen katsomatta on oikeus saada luotettavaa tietoa ja nähdä mediassa moninaista kuvaa ihmisyydestä, pohtii Havumäki näkökulmassaan.</description>
          <pubDate>Mon, 11 Oct 2021 11:02:00 +0300</pubDate>
          <category>journalismi</category><category>Näkökulma</category><category>tasa-arvo</category><category>sosiaalinen media</category><category>media</category><category>Näkökulmat</category><category>medialukutaito</category><category>Jouko Jokinen</category><category>lapset ja nuoret</category><category>tytöt</category><category>kolumnit</category><category>lapset</category><category>luotettavuus</category><category>nuoret</category><category>verkkoviestintä</category><category>Plan</category><category>perheet</category><category>Huono palvelu</category><category>ihmiskunta</category><category>sukupuoli</category><category>itsetunto</category>
          <guid isPermaLink=""false"">https://yle.fi/a/3-12127507</guid>
          <enclosure url=""https://images.cdn.yle.fi/image/upload//w_205,h_115,q_70/13-3-12110040.jpg"" type=""image/jpeg"" length=""0""/>
        </item>
    </channel>
</rss>
";

    [Fact]
    public async Task Some()
    {
        // arrange
        var rss = XDocument.Parse(_rssSome);
        var handler = new YleRssParseQueryHandler();
        // act
        var result = await handler.Handle(new YleRssParseQuery { Rss = rss }, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);
        Assert.Equal("https://yle.fi/a/74-20052790", result[0]);
        Assert.Equal("https://yle.fi/a/74-20052777", result[1]);
        Assert.Equal("https://yle.fi/a/3-12127507", result[2]);
    }
}
