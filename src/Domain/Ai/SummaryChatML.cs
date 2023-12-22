namespace AJE.Domain.Ai;

public class SummaryChatML : ChatMLCreator
{
    public const int VERSION = 1;
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
            "You are an assistant that examines given context and generates detailed summary of it",
            "Leave peoples opinions out of the summary if there are facts to report"
        ];

    public SummaryChatML() :
        base(EntityName, SystemInstructions)
    {
    }
}
