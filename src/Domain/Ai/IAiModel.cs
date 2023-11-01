
namespace AJE.Domain.Ai;

public interface IAiModel
{
    Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken);
    Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, Stream outputStream, CancellationToken cancellationToken);
    Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken);
    Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken);
    Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken);
}
