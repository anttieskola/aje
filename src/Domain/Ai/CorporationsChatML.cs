namespace AJE.Domain.Ai;

public interface ICorporations : IPromptCreator
{
    EquatableList<Corporation> Parse(string text);
}


public class CorporationsChatML : ChatMLCreator, ICorporations
{
    public const int VERSION = 1;

    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are an assistant that is tasked to examine context and list all corporations mentioned in the text",
        "Organisations (non-profit) is not a corporation",
        "In response list one corporation per line without any numbering or bullet points"
        ];

    public CorporationsChatML()
        : base(EntityName, SystemInstructions)
    {
    }

    public EquatableList<Corporation> Parse(string text)
    {
        var corporations = new EquatableList<Corporation>();
        foreach (var line in text.Split('\n'))
        {
            corporations.Add(new Corporation
            {
                Name = line.Trim()
            });
        }
        return corporations;
    }
}