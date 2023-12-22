using AJE.Domain.Ai;
using AJE.Domain.Entities;

namespace AJE.Test.Unit;

public class MarkDownLinkGathererTests
{
    [Fact]
    public async Task Ok()
    {
        var lg = new MarkDownLinkGatherer();
        var links = await lg.GetLinksAsync(
        [
            new MarkdownTextElement
            {
                Text = "*The All Points North podcast went chasing the northern lights in Finnish Lapland. Listen to the episode via this embedded player, on* [*Yle Areena*](https://areena.yle.fi/podcastit/1-4355773) *via* [*Apple*](https://podcasts.apple.com/us/podcast/all-points-north/id1678541537) *or* [*Spotify*](https://open.spotify.com/show/11M4NJ3cfmNCo0qYiIXXU1) *or wherever you get your podcasts.*",
            },
        ], CancellationToken.None);
        Assert.Equal(3, links.Count);
    }
}
