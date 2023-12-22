using AJE.Domain.Entities;
using AJE.Domain.Extensions;

namespace AJE.Test.Unit.Domain.Extesions;

public class ArticleExtensionTests
{
    [Fact]
    public void Exceptions()
    {
        Assert.Throws<ArgumentException>(() => new Article().GetContextForAnalysis());
        Assert.Throws<ArgumentException>(() => new Article { IsValidForAnalysis = true, TitleInEnglish = "title" }.GetContextForAnalysis());
        Assert.Throws<ArgumentException>(() => new Article { IsValidForAnalysis = true, ContentInEnglish = "content" }.GetContextForAnalysis());
        Assert.Throws<ArgumentException>(() => new Article { IsValidForAnalysis = false, TitleInEnglish = "title", ContentInEnglish = "content" }.GetContextForAnalysis());
        new Article { IsValidForAnalysis = true, TitleInEnglish = "title", ContentInEnglish = "content" }.GetContextForAnalysis();
    }


    [Fact]
    public void OK()
    {
        var article = new Article
        {
            IsValidForAnalysis = true,
            TitleInEnglish = "Title",
            ContentInEnglish = "Content",

            Persons =
            [
                new() {
                    Name = "Martti Ahtisaari",
                    ContentInEnglish = "Ahtisaari was a United Nations special envoy for Kosovo, charged with organizing the Kosovo status process negotiations."
                }
            ],

            Links =
            [
                new() {
                    Name = "SomeReferenceLink",
                    Uri = new Uri("https://en.wikipedia.org/wiki/Martti_Ahtisaari"),
                    ContentInEnglish = "Ahtisaari began his diplomatic career in 1973."
                }
            ]
        };

        var context = article.GetContextForAnalysis();
        Assert.NotNull(context);
        Assert.Contains("Title", context);
        Assert.Contains("Content", context);
        Assert.Contains("Martti Ahtisaari", context);
        Assert.Contains("Ahtisaari was a United Nations special envoy for Kosovo, charged with organizing the Kosovo status process negotiations.", context);
        Assert.Contains("SomeReferenceLink", context);
        Assert.DoesNotContain("https://en.wikipedia.org/wiki/Martti_Ahtisaari", context);
        Assert.Contains("Ahtisaari began his diplomatic career in 1973.", context);
    }
}
