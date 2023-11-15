﻿using AJE.Domain.Exceptions;

namespace AJE.Domain.Commands;

public record SendAiChatMessageCommand : IRequest<AiChatEvent>
{
    public required Guid ChatId { get; init; }

    public required string Message { get; init; }

    public required Stream Output { get; init; }
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
        var prompt = _antai.Chat(command.Message)
            ?? throw new AiException($"Failed to chat with message {command.Message}");
        var completionRequest = new CompletionRequest
        {
            Prompt = prompt,
            Stream = true,
        };
        var response = await _aiModel.CompletionStreamAsync(completionRequest, command.Output, cancellationToken);
        // TODO: Timestamp from the model response?
        var historyEntry = new AiChatHistoryEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            Input = command.Message,
            Output = response.Content.Trim(),
        };
        chat = await _aiChatRepository.AddHistoryEntry(chat.Id, historyEntry);
        var chatEvent = new AiChatMessageEvent
        {
            Id = chat.Id,
            Timestamp = historyEntry.Timestamp,
            Input = historyEntry.Input,
            Output = historyEntry.Output,
        };
        await _aiChatEventHandler.SendAsync(chatEvent);
        return chatEvent;
    }
}
