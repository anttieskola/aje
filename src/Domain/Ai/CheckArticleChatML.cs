namespace AJE.Domain.Ai;

public interface ICheckArticle : IPromptCreator
{
    bool Parse(string text);
    string ParseReasoning(string text);
}

public class CheckArticleChatML : ChatMLCreator, ICheckArticle
{
    public const string EntityName = "assistant";

    // update CURRENT_POLARITY_VERSION if system instructions change
    public static readonly string[] SystemInstructions = {
            "You are an assistant that examines given context and determinate is it a real article like news article or story about something or not",
            "You will respond using only one single word that is either yes or no then add line feed and resoning for your answer",
        };

    public CheckArticleChatML() :
        base(EntityName, SystemInstructions)
    {
    }

    public bool Parse(string text)
    {
        if (text == null || !text.Contains('\n'))
        {
            throw new AiException($"Invalid response:{text}");
        }
        var toParse = text[..text.IndexOf('\n')];
        if (toParse.Contains("yes", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else if (toParse.Contains("no", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        throw new AiException($"Invalid response:{text}");
    }

    public string ParseReasoning(string text)
    {
        if (text == null || !text.Contains('\n'))
        {
            throw new AiException($"Invalid response:{text}");
        }
        return text[(text.IndexOf('\n') + 1)..];
    }
}
