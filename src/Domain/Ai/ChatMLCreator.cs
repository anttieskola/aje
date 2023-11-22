﻿namespace AJE.Domain.Ai;

public abstract class ChatMLCreator : IPromptCreator
{
    protected readonly string _entityName;
    protected readonly string[] _systemInstructions;

    protected ChatMLCreator(string entityName, string[] systemInstructions)
    {
        _entityName = entityName;
        _systemInstructions = systemInstructions;
    }

    private readonly string _iStart = "<|im_start|>";
    private readonly string _iEnd = "<|im_end|>";

    #region IPromptCreator

    public string[] StopWords => new string[] { _iStart, _iEnd };

    public string Context(string context)
    {
        var sb = new StringBuilder();
        sb.Append(_iStart);
        sb.Append("system\n");
        foreach (var instruction in _systemInstructions)
        {
            sb.Append(instruction);
            sb.Append('\n');
        }
        sb.Append(_iEnd);
        sb.Append(_iStart);
        sb.Append("context\n");
        sb.Append(context);
        sb.Append(_iEnd);
        sb.Append(_iStart);
        sb.Append(_entityName);
        sb.Append('\n');
        return sb.ToString();
    }

    public string Chat(string message)
    {
        var sb = new StringBuilder();
        sb.Append(_iStart);
        sb.Append("system\n");
        foreach (var instruction in _systemInstructions)
        {
            sb.Append(instruction);
            sb.Append('\n');
        }
        sb.Append(_iEnd);
        sb.Append(_iStart);
        sb.Append("user\n");
        sb.Append(message);
        sb.Append(_iEnd);
        sb.Append(_iStart);
        sb.Append(_entityName);
        sb.Append('\n');
        return sb.ToString();
    }

    public string Chat(string message, AiChatInteractionEntry[] interactions)
    {
        var sb = new StringBuilder();
        sb.Append(_iStart);
        sb.Append("system\n");
        foreach (var instruction in _systemInstructions)
        {
            sb.Append(instruction);
            sb.Append('\n');
        }
        sb.Append(_iEnd);
        foreach (var entry in interactions)
        {
            sb.Append(_iStart);
            sb.Append("user\n");
            sb.Append(entry.Input);
            sb.Append(_iEnd);
            sb.Append(_iStart);
            sb.Append(_entityName);
            sb.Append('\n');
            sb.Append(entry.Output);
            sb.Append(_iEnd);
        }
        sb.Append(_iStart);
        sb.Append("user\n");
        sb.Append(message);
        sb.Append(_iEnd);
        sb.Append(_iStart);
        sb.Append(_entityName);
        sb.Append('\n');
        return sb.ToString();
    }

    #endregion IPromptCreator
}