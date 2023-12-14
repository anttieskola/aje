namespace AJE.Domain.Queries;

public class PositiveThingsChatML : ChatMLCreator
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
            "You are an assistant that generates a list of positive things from the given context in english"
        ];

    public PositiveThingsChatML()
        : base(EntityName, SystemInstructions)
    {
    }
}
