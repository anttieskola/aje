namespace AJE.Domain.Ai;

public class MarkDownSimplifier : ISimplifier
{
    public string Simplify(string content)
    {
        var sb = new StringBuilder();
        bool inLink = false;
        bool inCodeBlock = false;

        for (var i = 0; i < content.Length; i++)
        {
            char current = content[i];
            char? next = i + 1 < content.Length ? content[i + 1] : null;
            char? previous = i - 1 >= 0 ? content[i - 1] : null;

            // skip links
            if (current == '(' && previous == ']')
            {
                inLink = true;
                continue;
            }
            else if (current == ')' && inLink)
            {
                inLink = false;
                continue;
            }
            // skip code blocks
            else if (previous == '`' && current == '`' && next == '`')
            {
                inCodeBlock = !inCodeBlock;
                continue;
            }
            // remove extra spaces
            else if (current == ' ' && previous == ' ')
            {
                continue;
            }

            if (!inLink
                && !inCodeBlock
                && IsValidChar(current))
            {
                sb.Append(current);
            }
        }
        return sb.ToString().Trim();
    }

    private static bool IsValidChar(char c)
    {
        return char.IsLetter(c) || char.IsDigit(c)
            || c == ' '
            || c == '-'
            || c == '.'
            || c == ',';
    }
}
