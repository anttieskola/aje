namespace AJE.Domain.Ai;

public class SummaryOfSummariesChatML : ChatMLCreator
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
            "You are an assistant that examines given context that is a list of news article titles and their summaries and generates one detailed summary from all of it",
        ];

    public SummaryOfSummariesChatML() :
        base(EntityName, SystemInstructions)
    {
    }
}
