namespace AJE.Domain.Ai;

public interface IOrganizations : IPromptCreator
{
    EquatableList<Organization> Parse(string text);
}


public class OrganizationsChatML : ChatMLCreator, IOrganizations
{
    public const int VERSION = 1;

    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = [
        "You are an assistant that is tasked to examine context and list all organizations mentioned in the text",
        "Corporation which is for profit is not an organization",
        "Country is not an organization",
        "In response list one organization per line without any numbering or bullet points"
        ];

    public OrganizationsChatML()
        : base(EntityName, SystemInstructions)
    {
    }

    public EquatableList<Organization> Parse(string text)
    {
        var organizations = new EquatableList<Organization>();
        foreach (var line in text.Split('\n'))
        {
            organizations.Add(new Organization
            {
                Name = line.Trim()
            });
        }
        return organizations;
    }
}