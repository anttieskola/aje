﻿namespace AJE.Domain.Ai;

public interface IPolarity : IPromptCreator
{
    Polarity Parse(string text);
}

public class PolarityChatML : ChatMLCreator, IPolarity
{
    public const string EntityName = "assistant";

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