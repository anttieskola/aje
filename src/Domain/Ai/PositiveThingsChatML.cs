namespace AJE.Domain.Queries;

public class PositiveThingsChatML : ChatMLCreator
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are an assistant that examines given context and finds all positive things about it",
        "Price increases that affect working-class is not positive",
        "Anything that reduces people drinking alcohol is positive, also selling less alcohol is positive",
        "Anything that reduces people smoking normal cigarettes is positive, also selling less cigarettes is positive",
        "Death from un-natural causes is never positive"
        ];

    public PositiveThingsChatML()
        : base(EntityName, SystemInstructions)
    {
    }
}
