namespace AJE.Domain.Queries;

public record ArticlePrepForAnalysisQuery : IRequest<Article>
{
    public required Article Article { get; init; }
}

/// <summary>
/// Prepares article for analysis (no AI use yet here)
/// </summary>
public class ArticlePrepForAnalysisHandler : IRequestHandler<ArticlePrepForAnalysisQuery, Article>
{
    private readonly string[] _supportedLanguages = ["en", "fi", "sv", "ru"];
    private readonly IContextCreator<Article> _contextCreator;
    private readonly ITranslate _translate;
    private readonly IPersonGatherer _personGatherer;
    private readonly ILinkGatherer _linkGatherer;

    public ArticlePrepForAnalysisHandler(
        IContextCreator<Article> contextCreator,
        ITranslate translate,
        IPersonGatherer personGatherer,
        ILinkGatherer linkGatherer)
    {
        _contextCreator = contextCreator;
        _translate = translate;
        _personGatherer = personGatherer;
        _linkGatherer = linkGatherer;
    }

    public async Task<Article> Handle(ArticlePrepForAnalysisQuery query, CancellationToken cancellationToken)
    {
        if (!_supportedLanguages.Contains(query.Article.Language))
        {
            throw new ArgumentException($"Language {query.Article.Language} not supported for analysis.");
        }

        // create context and translate if needed
        var titleInEnglish = query.Article.Title;
        var contentInEnglish = _contextCreator.Create(query.Article);
        if (query.Article.Language != "en")
        {
            var titleTranslateResponse = await _translate.TranslateAsync(new TranslateRequest
            {
                SourceLanguage = query.Article.Language,
                TargetLanguage = "en",
                Text = query.Article.Title,
            }, cancellationToken);
            titleInEnglish = titleTranslateResponse.TranslatedText;

            var contentTranslateResponse = await _translate.TranslateAsync(new TranslateRequest
            {
                SourceLanguage = query.Article.Language,
                TargetLanguage = "en",
                Text = contentInEnglish,
            }, cancellationToken);
            contentInEnglish = contentTranslateResponse.TranslatedText;
        }

        // Gather persons
        var persons = await _personGatherer.GetPersonsAsync(query.Article.Content, cancellationToken);

        // Gather links
        var links = await _linkGatherer.GetLinksAsync(query.Article.Content, cancellationToken);

        return query.Article with
        {
            IsValidForAnalysis = true,
            TitleInEnglish = titleInEnglish,
            ContentInEnglish = contentInEnglish,
            Persons = persons,
            Links = links,
        };
    }
}