namespace AJE.Domain.Ai;

public class PromptStudioInstructML : InstructMLCreator
{
    public void SetEntityName(string entityName)
    {
        _entityName = entityName;
    }

    public void SetSystemInstructions(string[] systemInstructions)
    {
        _systemInstructions = systemInstructions;
    }
}
