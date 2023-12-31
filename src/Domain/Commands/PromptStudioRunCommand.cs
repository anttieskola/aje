﻿namespace AJE.Domain.Commands;

public record PromptStudioRunCommand : IRequest<PromptStudioRunCompletedEvent>
{
    public bool IsTest { get; init; } = false;
    public AiSyntax Syntax { get; init; } = AiSyntax.ChatML;
    public required Guid SessionId { get; init; }
    public required Guid RunId { get; init; }
    public required string EntityName { get; init; }
    public required string[] SystemInstructions { get; init; }
    public required string Context { get; init; }
    public required double Temperature { get; init; }
    public required int NumberOfTokensToPredict { get; init; }
}

public class PromptStudioRunCommandHandler : IRequestHandler<PromptStudioRunCommand, PromptStudioRunCompletedEvent>
{
    private readonly IPromptStudioRepository _promptStudioRepository;
    private readonly IPromptStudioEventHandler _promptStudioEventHandler;
    private readonly PromptStudioChatML _promptStudioChatML = new();
    private readonly PromptStudioInstructML _promptStudioInstructML = new();
    private readonly IAiModel _aiModel;

    public PromptStudioRunCommandHandler(
        IPromptStudioRepository promptStudioRepository,
        IPromptStudioEventHandler promptStudioEventHandler,
        IAiModel aiModel)
    {
        _promptStudioRepository = promptStudioRepository;
        _promptStudioEventHandler = promptStudioEventHandler;
        _aiModel = aiModel;
    }

    public async Task<PromptStudioRunCompletedEvent> Handle(PromptStudioRunCommand command, CancellationToken cancellationToken)
    {
        // get session
        var session = await _promptStudioRepository.GetAsync(command.SessionId)
            ?? throw new KeyNotFoundException($"Session with id {command.SessionId} not found");

        // create prompt
        _promptStudioChatML.SetEntityName(command.EntityName);
        _promptStudioInstructML.SetEntityName(command.EntityName);
        _promptStudioChatML.SetSystemInstructions(command.SystemInstructions);
        _promptStudioInstructML.SetSystemInstructions(command.SystemInstructions);

        string prompt = string.Empty;
        prompt = command.Syntax switch
        {
            AiSyntax.ChatML => _promptStudioChatML.Context(command.Context)
                                ?? throw new AiException($"Failed to create context for PromptStudio message:{command.Context}"),
            AiSyntax.InstructML => _promptStudioInstructML.Context(command.Context)
                                ?? throw new AiException($"Failed to create context for PromptStudio message:{command.Context}"),
            _ => throw new AiException($"Unsupported syntax: {command.Syntax}"),
        };

        // callback on tokens
        async Task OnTokenCreated(string token)
        {
            await _promptStudioEventHandler.SendAsync(new PromptStudioRunTokenEvent
            {
                IsTest = command.IsTest,
                SessionId = command.SessionId,
                RunId = command.RunId,
                EventTimeStamp = DateTimeOffset.UtcNow,
                Token = token,
            });
        }

        // ai request
        var completionRequest = new CompletionRequest
        {
            Prompt = prompt,
            Temperature = command.Temperature,
            Stop = _promptStudioChatML.StopWords,
            NumberOfTokensToPredict = command.NumberOfTokensToPredict,
            Stream = true,
        };
        var response = await _aiModel.CompletionStreamAsync(completionRequest, OnTokenCreated, cancellationToken);

        // store run
        var systemInstructions = new EquatableList<string>();
        systemInstructions.AddRange(command.SystemInstructions);
        var runEntry = new PromptStudioRun
        {
            RunId = command.RunId,
            EntityName = command.EntityName,
            SystemInstructions = systemInstructions,
            Context = command.Context,
            Result = response.Content,
            Model = Path.GetFileNameWithoutExtension(response.GenerationSettings.Model),
            NumberOfTokensEvaluated = response.TokensEvaluated,
            NumberOfTokensContext = response.GenerationSettings.NumberOfTokensContext,
        };
        session = await _promptStudioRepository.AddRunAsync(session.SessionId, runEntry);

        // send run completed event
        var runCompletedEvent = new PromptStudioRunCompletedEvent
        {
            IsTest = command.IsTest,
            SessionId = session.SessionId,
            EventTimeStamp = DateTimeOffset.UtcNow,
            RunId = runEntry.RunId,
            Input = runEntry.Context,
            Output = runEntry.Result,
            Model = runEntry.Model,
            NumberOfTokensEvaluated = runEntry.NumberOfTokensEvaluated,
            NumberOfTokensContext = runEntry.NumberOfTokensContext,
        };
        await _promptStudioEventHandler.SendAsync(runCompletedEvent);
        return runCompletedEvent;
    }
}