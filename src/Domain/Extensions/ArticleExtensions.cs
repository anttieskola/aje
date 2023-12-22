namespace AJE.Domain.Extensions;

public static class ArticleExtensions
{
    public static string GetContextForAnalysis(this Article article)
    {
        if (!article.IsValidForAnalysis)
            throw new ArgumentException("Article IsValidForAnalysis is false");

        ArgumentException.ThrowIfNullOrWhiteSpace(article.TitleInEnglish);
        ArgumentException.ThrowIfNullOrWhiteSpace(article.ContentInEnglish);

        var sb = new StringBuilder();

        // title
        sb.AppendLine(article.TitleInEnglish);

        // persons
        foreach (var person in article.Persons.Where(p => !string.IsNullOrWhiteSpace(p.ContentInEnglish)))
        {
            sb.AppendLine(person.Name);
            sb.AppendLine(person.ContentInEnglish);
        }

        // links
        foreach (var link in article.Links.Where(l => !string.IsNullOrWhiteSpace(l.ContentInEnglish)))
        {
            sb.AppendLine(link.Name);
            sb.AppendLine(link.ContentInEnglish);
        }

        // content
        sb.AppendLine(article.ContentInEnglish);

        return sb.ToString();
    }
}
