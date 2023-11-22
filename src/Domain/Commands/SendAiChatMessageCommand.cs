using AJE.Domain.Exceptions;

namespace AJE.Domain.Commands;

public record SendAiChatMessageCommand : IRequest<AiChatEvent>
{
    public bool IsTest { get; init; } = false;
    public required Guid ChatId { get; init; }
    public required string Message { get; init; }
}

public class SendAiChatMessageCommandHandler : IRequestHandler<SendAiChatMessageCommand, AiChatEvent>
{
    private readonly IAiChatRepository _aiChatRepository;
    private readonly IAiChatEventHandler _aiChatEventHandler;
    private readonly IAntai _antai;
    private readonly IAiModel _aiModel;

    public SendAiChatMessageCommandHandler(
        IAiChatRepository aiChatRepository,
        IAiChatEventHandler aiChatEventHandler,
        IAntai antai,
        IAiModel aiModel)
    {
        _aiChatRepository = aiChatRepository;
        _aiChatEventHandler = aiChatEventHandler;
        _antai = antai;
        _aiModel = aiModel;
    }

    public async Task<AiChatEvent> Handle(SendAiChatMessageCommand command, CancellationToken cancellationToken)
    {
        var chat = await _aiChatRepository.GetAsync(command.ChatId)
            ?? throw new KeyNotFoundException($"Chat with id {command.ChatId} not found");
        var prompt = _antai.Chat(command.Message, chat.Interactions.ToArray())
            ?? throw new AiException($"Failed to create context for Antai message:{command.Message}");

        // send event for each token created
        async Task OnTokenCreated(string token)
        {
            await _aiChatEventHandler.SendAsync(new AiChatTokenEvent
            {
                IsTest = command.IsTest,
                ChatId = command.ChatId,
                EventTimeStamp = DateTimeOffset.UtcNow,
                Token = token,
            });
        }

        // TODO: Token calculation using tokenizer API, to prevent hogging all GPU resources
        var completionRequest = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = 0.2,
            Stop = _antai.StopWords,
            Stream = true,
        };
        var response = await _aiModel.CompletionStreamAsync(completionRequest, OnTokenCreated, cancellationToken);
        // TODO: Timestamp from the model response?
        // TODO: Take token calculation
        var historyEntry = new AiChatInteractionEntry
        {
            InteractionId = Guid.NewGuid(),
            InteractionTimestamp = DateTimeOffset.UtcNow,
            Input = command.Message,
            Output = response.Content.Trim(),
        };
        chat = await _aiChatRepository.AddHistoryEntry(chat.ChatId, historyEntry);
        var chatEvent = new AiChatInteractionEvent
        {
            IsTest = command.IsTest,
            ChatId = chat.ChatId,
            EventTimeStamp = historyEntry.InteractionTimestamp,
            InteractionId = historyEntry.InteractionId,
            Input = historyEntry.Input,
            Output = historyEntry.Output,
        };
        await _aiChatEventHandler.SendAsync(chatEvent);
        return chatEvent;
    }
}
