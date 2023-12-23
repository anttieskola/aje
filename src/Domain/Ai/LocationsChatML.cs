namespace AJE.Domain.Ai;

public interface ILocations : IPromptCreator
{
    EquatableList<Location> Parse(string text);
}

public class LocationsChatML : ChatMLCreator, ILocations
{
    public const int VERSION = 1;

    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are an assistant that is tasked to examine context and list all geographical locations mentioned in the text",
        "In response list one location per line without any numbering or bullet points"
        ];

    public LocationsChatML()
        : base(EntityName, SystemInstructions)
    {
    }

    public EquatableList<Location> Parse(string text)
    {
        var locations = new EquatableList<Location>();
        foreach (var line in text.Split('\n'))
        {
            locations.Add(new Location
            {
                Name = line.Trim()
            });
        }
        return locations;
    }
}
