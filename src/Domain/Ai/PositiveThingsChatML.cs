namespace AJE.Domain.Queries;

public interface IPositiveThings : IPromptCreator
{
    EquatableList<PositiveThing> Parse(string text);
}

public class PositiveThingsChatML : ChatMLCreator, IPositiveThings
{
    public const int VERSION = 2;

    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are an assistant that examines given context and finds all positive things about given context using only the given context and not prior knowledge but using following guidelines",
        "Price increases that affect working-class is not positive",
        "Anything that reduces people drinking alcohol is positive, also selling less alcohol is positive",
        "Anything that reduces people smoking normal cigarettes is positive, also selling less cigarettes is positive",
        "Death from un-natural causes is never positive",
        "Write output as JSON array of objects where objects have title and description",
        ];

    public PositiveThingsChatML()
        : base(EntityName, SystemInstructions)
    {
    }

    public EquatableList<PositiveThing> Parse(string text)
    {
        var json = text.Trim();
        try
        {
            var things = JsonSerializer.Deserialize<EquatableList<PositiveThing>>(json) ?? throw new AiParseException("Failed to parse response");
            return things;
        }
        catch (Exception ex)
        {
            throw new AiParseException(ex.Message);
        }
    }
}
