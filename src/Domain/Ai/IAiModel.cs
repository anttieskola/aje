
namespace AJE.Domain.Ai;

public delegate Task TokenCreatedCallback(string token);

public interface IAiModel
{
    Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken);
    Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken);
    Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken);
    Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken);
    Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken);
    Task<int> MaxTokenCountAsync(CancellationToken cancellationToken);
}
