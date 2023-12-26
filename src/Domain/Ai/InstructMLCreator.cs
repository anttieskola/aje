namespace AJE.Domain.Ai;

/// <summary>
/// <s> [INST] Instruction [/INST] Model answer</s> [INST] Follow-up instruction [/INST]
///
/// Note that <s> and </s> are special tokens for beginning of string (BOS) and end of string (EOS)
/// while [INST] and [/INST] are regular strings.
/// </summary>
public abstract class InstructMLCreator : IPromptCreator
{
    protected string _entityName = string.Empty;
    protected string[] _systemInstructions = [];

    protected InstructMLCreator()
    {
    }

    protected InstructMLCreator(string entityName, string[] systemInstructions)
    {
        _entityName = entityName;
        _systemInstructions = systemInstructions;
    }

    private readonly string _iStart = "[INST]";
    private readonly string _iEnd = "[/INST]";

    public string[] StopWords => [_iStart, _iEnd];

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
        throw new NotImplementedException();
    }

    public string Chat(string message, AiChatInteractionEntry[] interactions)
    {
        throw new NotImplementedException();
    }
}
