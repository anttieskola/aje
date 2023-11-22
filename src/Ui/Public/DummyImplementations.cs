namespace AJE.Ui.Public;

public class DummyAiModel : IAiModel
{
    public Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CompletionResponse> CompletionStreamAsync(CompletionRequest request, TokenCreatedCallback tokenCreatedCallback, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<DeTokenizeResponse> DeTokenizeAsync(DeTokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<EmbeddingResponse> EmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> MaxTokenCountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TokenizeResponse> TokenizeAsync(TokenizeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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

    public Task<AiChat> AddInteractionEntryAsync(Guid chatId, AiChatInteractionEntry entry)
    {
        throw new NotImplementedException();
    }

    public Task<AiChat> GetAsync(Guid chatId)
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

    public void Subscribe(Guid subscriberId, Func<AiChatEvent, Task> handler)
    {
        throw new NotImplementedException();
    }

    public void SubscribeToChat(Guid subscriberId, Guid chatId, Func<AiChatEvent, Task> handler)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(Guid subscriberId)
    {
        throw new NotImplementedException();
    }
}