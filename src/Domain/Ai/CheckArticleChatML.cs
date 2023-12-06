namespace AJE.Domain.Ai;

public record CheckArticleChatMLResult
{
    public required bool IsValid { get; init; }
    public required string Reasoning { get; init; }
}

public interface ICheckArticle : IPromptCreator
{
    CheckArticleChatMLResult Parse(string text);
}

public class CheckArticleChatML : ChatMLCreator, ICheckArticle
{
    public const string EntityName = "assistant";

    // update CURRENT_POLARITY_VERSION if system instructions change
    public static readonly string[] SystemInstructions = {
            "You are an assistant that examines given context and determinate is it a valid article or not",
            "Valid articles are those that report news, events, facts or statistics",
            "Valid articles must contain atleast 40 words",
            "You response using single word YES or NO followed by reasoning for the answer"
        };

    public CheckArticleChatML() :
        base(EntityName, SystemInstructions)
    {
    }

    public CheckArticleChatMLResult Parse(string text)
    {
        if (text == null)
            throw new AiException($"Invalid response:{text}");

        // remove possible line feed or space from the beginning
        text = text.TrimStart();
        string reasoning;
        bool isValid;
        if (text.StartsWith("yes", StringComparison.OrdinalIgnoreCase) || (text.Length > 30 && text[..30].Contains("yes", StringComparison.OrdinalIgnoreCase)))
        {
            isValid = true;
            reasoning = text[3..];
        }
        else if (text.StartsWith("no", StringComparison.OrdinalIgnoreCase) || (text.Length > 30 && text[..30].Contains("no", StringComparison.OrdinalIgnoreCase)))
        {
            isValid = false;
            reasoning = text[2..].Trim();
        }
        else
        {
            throw new AiException($"Invalid response:{text}");
        }

        return new CheckArticleChatMLResult
        {
            IsValid = isValid,
            Reasoning = reasoning,
        };
    }
}
