namespace AJE.Domain.Translate;

public interface ITranslate
{
    Task<TranslateResponse> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken);
}
