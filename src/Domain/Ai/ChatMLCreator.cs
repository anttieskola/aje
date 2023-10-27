﻿namespace AJE.Domain.Ai;

public enum HistoryEntryType
{
    User = 2,
    Entity = 3
}

public record HistoryEntry
{
    public required HistoryEntryType Type { get; init; }
    public required string Text { get; init; }
}

public abstract class ChatMLCreator : IPromptCreator
{
    protected readonly string _entityName;
    protected readonly string[] _systemInstructions;

    public ChatMLCreator(string entityName, string[] systemInstructions)
    {
        _entityName = entityName;
        _systemInstructions = systemInstructions;
    }

    // TODO: history support
    /*
    public ChatMLCreator(
        string entityName,
        string[] systemInstructions,
        IEnumerable<HistoryEntry> history)
    {

    }
    */

    private string _iStart = "<|im_start|>";
    private string _iEnd = "<|im_end|>";

    #region IPromptCreator

    public string[] StopWords => new string[] { _iStart, _iEnd };

    public string Create(string context)
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

    #endregion IPromptCreator
}