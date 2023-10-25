namespace AJE.Domain.Ai;

public interface IPolarity : IPromptCreator
{
    Polarity Parse(string text);
}

public class PolarityChatML : ChatMLCreator, IPolarity
{
    public const string EntityName = "assistant";

    // TODO: Odd responses from model, need to investigate
    // - dramatic / article:7d5844ce-825a-4c39-a933-587be0d3459a / 74-20054239
    // - challenging / article:83361d7b-ee2d-4ccd-aad4-d804908d675b / 74-20053674
    // - preparedness / article:ab35d487-c23b-4732-9f06-1dae6e1d034d / 74-20052916
    // - smuggling / article:c5930aae-723a-43ed-a34e-dd8665001069 / 74-20052871

    // update CURRENT_POLARITY_VERSION if system instructions change
    public static readonly string[] SystemInstructions = {
            "You are an assistant that classifies polarity of given context as neutral, positive or negative",
            "You will respond using only one single word that is either neutral, positive or negative"
        };

    public PolarityChatML() :
        base(EntityName, SystemInstructions)
    {
    }

    public Polarity Parse(string text)
    {
        if (text.Contains("positive", StringComparison.OrdinalIgnoreCase))
        {
            return Polarity.Positive;
        }
        else if (text.Contains("neutral", StringComparison.OrdinalIgnoreCase))
        {
            return Polarity.Neutral;
        }
        else if (text.Contains("negative", StringComparison.OrdinalIgnoreCase))
        {
            return Polarity.Negative;
        }
        return Polarity.Unknown;
    }
}
