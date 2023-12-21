namespace AJE.Domain.Queries;

public record TranslateArticleQuery : IRequest<Article>
{
    public required Article Article { get; init; }
    public string TargetLanguage { get; init; } = "en";
}

public class TranslateArticleQueryHandler : IRequestHandler<TranslateArticleQuery, Article>
{
    private readonly ITranslate _translate;

    public TranslateArticleQueryHandler(ITranslate translate)
    {
        _translate = translate;
    }

    public async Task<Article> Handle(TranslateArticleQuery query, CancellationToken cancellationToken)
    {
        var titleResponse = await _translate.TranslateAsync(new TranslateRequest
        {
            SourceLanguage = query.Article.Language,
            TargetLanguage = query.TargetLanguage,
            Text = query.Article.Title,
        }, cancellationToken);

        var translatedContent = new EquatableList<MarkdownElement>();
        foreach (var content in query.Article.Content)
        {
            var response = await _translate.TranslateAsync(new TranslateRequest
            {
                SourceLanguage = query.Article.Language,
                TargetLanguage = query.TargetLanguage,
                Text = content.Text,
            }, cancellationToken);

            switch (content)
            {
                case MarkdownHeaderElement header:
                    translatedContent.Add(new MarkdownHeaderElement
                    {
                        Level = header.Level,
                        Text = response.TranslatedText,
                    });
                    break;
                case MarkdownTextElement:
                    translatedContent.Add(new MarkdownTextElement
                    {
                        Text = response.TranslatedText,
                    });
                    break;
                default:
                    throw new PlatformException($"Unknown MarkdownElement type {content.GetType().Name}");
            }

        }
        var translatedArticle = query.Article with { Content = translatedContent, Title = titleResponse.TranslatedText, Language = query.TargetLanguage };
        return translatedArticle;
    }
}