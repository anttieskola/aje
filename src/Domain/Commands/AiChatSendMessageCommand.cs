namespace AJE.Domain.Commands;

public record AiChatSendMessageCommand : IRequest<AiChatEvent>
{
    public bool IsTest { get; init; } = false;
    public required Guid ChatId { get; init; }
    public required string Message { get; init; }
}

public class AiChatSendMessageCommandHandler : IRequestHandler<AiChatSendMessageCommand, AiChatEvent>
{
    private readonly IAiChatRepository _aiChatRepository;
    private readonly IAiChatEventHandler _aiChatEventHandler;
    private readonly IAntai _antai;
    private readonly IAiModel _aiModel;

    public AiChatSendMessageCommandHandler(
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

    public async Task<AiChatEvent> Handle(AiChatSendMessageCommand command, CancellationToken cancellationToken)
    {
        var chat = await _aiChatRepository.GetAsync(command.ChatId)
            ?? throw new KeyNotFoundException($"Chat with id {command.ChatId} not found");
        var prompt = _antai.Chat(command.Message, chat.Interactions.ToArray())
            ?? throw new AiException($"Failed to create context for Antai message:{command.Message}");

        // request
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
        var interactionEntry = new AiChatInteractionEntry
        {
            InteractionId = Guid.NewGuid(),
            InteractionTimestamp = DateTimeOffset.UtcNow,
            Input = command.Message,
            Output = response.Content.Trim(),
            Model = response.GenerationSettings.Model,
            NumberOfTokensEvaluated = response.TokensEvaluated,
            NumberOfTokensContext = response.GenerationSettings.NumberOfTokensContext,
        };
        chat = await _aiChatRepository.AddInteractionEntryAsync(chat.ChatId, interactionEntry);
        var interactionEvent = new AiChatInteractionEvent
        {
            IsTest = command.IsTest,
            ChatId = chat.ChatId,
            EventTimeStamp = interactionEntry.InteractionTimestamp,
            InteractionId = interactionEntry.InteractionId,
            Input = interactionEntry.Input,
            Output = interactionEntry.Output,
            Model = interactionEntry.Model,
            NumberOfTokensEvaluated = interactionEntry.NumberOfTokensEvaluated,
            NumberOfTokensContext = interactionEntry.NumberOfTokensContext,
        };
        await _aiChatEventHandler.SendAsync(interactionEvent);
        return interactionEvent;
    }
}
