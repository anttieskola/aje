namespace AJE.Domain.Ai;

public interface IPolarity : IPromptCreator
{
    Polarity Parse(string text);
}

public class PolarityChatML : ChatMLCreator, IPolarity
{
    public const string EntityName = "assistant";

    public static readonly string[] SystemInstructions = {
        "You are an assistant that classifies polarity of given context as neutral, positive or negative",
        "Death from un-natural causes is always negative",
        "Death is positive when it is from natural causes and person lived long and successful life",
        "Price increases that affect working-class is always negative",
        "Anything that reduces people drinking alcohol is positive, also selling less alcohol is positive",
        "Anything that reduces people smoking normal cigarettes is positive, also selling less cigarettes is positive",
        "You will respond using only one single word that is either neutral, positive or negative",
        "When police detain people just for marching or opinions its always negative"
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
