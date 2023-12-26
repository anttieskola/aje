namespace AJE.Domain.Ai;

public interface IKeyPersons : IPromptCreator
{
    EquatableList<KeyPerson> Parse(string text);
}

public class KeyPeopleChatML : ChatMLCreator, IKeyPersons
{
    public const int VERSION = 1;

    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are assistant that examines the given context and from it extracts key natural persons, their role and intention mentioned using only provided context and not prior knowledge",
        "Write output in JSON array where each natural person is own object and field name is name, role is role and intention is intention",
        ];

    public KeyPeopleChatML()
        : base(EntityName, SystemInstructions)
    {
    }

    public EquatableList<KeyPerson> Parse(string text)
    {
        var json = text.Trim();
        try
        {
            var keyPersons = JsonSerializer.Deserialize<EquatableList<KeyPerson>>(json) ?? throw new AiParseException("Failed to parse response");
            return keyPersons;
        }
        catch (Exception ex)
        {
            throw new AiParseException(ex.Message);
        }
    }
}
