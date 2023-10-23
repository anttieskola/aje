namespace AJE.Domain.Ai;

public interface IPolarity : IPromptCreator
{
    Polarity Parse(string text);
}

public class PolarityChatMLCreator : ChatMLCreator, IPolarity
{
    public const string EntityName = "assistant";
    public static readonly string[] SystemInstructions = {
            "You are an assistant that classifies polarity of given context as neutral, positive or negative",
            "You will respond using only one single word that is either neutral, positive or negative"
        };

    public PolarityChatMLCreator() :
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
        else if (text.Equals("negative", StringComparison.OrdinalIgnoreCase))
        {
            return Polarity.Negative;
        }
        return Polarity.Unknown;
    }
}
