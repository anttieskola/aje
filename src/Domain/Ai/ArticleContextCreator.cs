namespace AJE.Domain.Ai;

public class ArticleContextCreator : IContextCreator<Article>
{
    private readonly ISimplifier _simplifier;

    public ArticleContextCreator(ISimplifier simplifier)
    {
        _simplifier = simplifier;
    }

    public string Create(Article article)
    {
        var sb = new StringBuilder();
        foreach (var element in article.Content)
        {
            if (element is MarkdownHeaderElement headerText)
            {
                sb.Append(_simplifier.Simplify(headerText.Text));
            }
            else if (element is MarkdownTextElement text)
            {
                sb.Append(_simplifier.Simplify(text.Text));
            }
        }
        return sb.ToString();
    }
}
