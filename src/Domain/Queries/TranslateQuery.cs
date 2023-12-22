namespace AJE.Domain.Queries;

public record TranslateQuery : IRequest<string>
{
    public required string Text { get; init; }
    public string SourceLanguage { get; init; } = "auto";
    public string TargetLanguage { get; init; } = "en";
}

public class TranslateQueryHandler : IRequestHandler<TranslateQuery, string>
{
    private readonly ITranslate _translate;

    public TranslateQueryHandler(ITranslate translate)
    {
        _translate = translate;
    }

    public async Task<string> Handle(TranslateQuery query, CancellationToken cancellationToken)
    {
        var response = await _translate.TranslateAsync(new TranslateRequest
        {
            SourceLanguage = query.SourceLanguage,
            TargetLanguage = query.TargetLanguage,
            Text = query.Text
        }, cancellationToken);
        return response.TranslatedText;
    }
}