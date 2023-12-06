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
            "You are an assistant that examines given context and determinate is it a real article, like news article or story about some event or a report of statistics",
            "You must answer with single word yes or no, then add comma or line feed and reasoning for the answer"
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
        string answer;
        string reasoning;
        if (text.Contains('\n'))
        {
            answer = text[..text.IndexOf('\n')];
            reasoning = text[(text.IndexOf('\n') + 1)..];
        }
        else if (text.Contains(','))
        {
            answer = text[..text.IndexOf(',')];
            reasoning = text[(text.IndexOf(',') + 1)..];
        }
        else
        {
            throw new AiException($"Invalid response:{text}");
        }

        bool isValid;
        if (answer.StartsWith("yes", StringComparison.OrdinalIgnoreCase))
        {
            isValid = true;
        }
        else if (answer.StartsWith("no", StringComparison.OrdinalIgnoreCase))
        {
            isValid = false;
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
