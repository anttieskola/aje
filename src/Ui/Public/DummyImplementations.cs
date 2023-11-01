namespace AJE.Ui.Public;

public class DummyAiModel : IAiModel
{
    public Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, Stream outputStream, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}

public class DummyAiLogger : IAiLogger
{
    public Task LogAsync(string fileNamePrefix, CompletionRequest request, CompletionResponse response)
    {
        throw new NotSupportedException();
    }
}