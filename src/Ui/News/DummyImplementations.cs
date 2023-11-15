namespace AJE.Ui.News;

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

public class DummyAiChatRepository : IAiChatRepository
{
    public Task<AiChat> AddAsync(AiChatOptions options)
    {
        throw new NotImplementedException();
    }

    public Task<AiChat> AddHistoryEntry(Guid id, AiChatInteractionEntry entry)
    {
        throw new NotImplementedException();
    }

    public Task<AiChat> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

public class DummyAiChatEventHandler : IAiChatEventHandler
{
    public Task SendAsync(AiChatEvent aiChatEvent)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<AiChatEvent> handler)
    {
        throw new NotImplementedException();
    }
}