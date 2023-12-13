namespace AJE.Domain.Ai;

public class SummaryChatML : ChatMLCreator
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
            "Generate very detailed summary of given context in english"
        ];

    public SummaryChatML() :
        base(EntityName, SystemInstructions)
    {
    }
}
