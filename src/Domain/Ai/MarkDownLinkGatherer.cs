﻿namespace AJE.Domain.Ai;

public partial class MarkDownLinkGatherer : ILinkGatherer
{
    public Task<EquatableList<Link>> GetLinksAsync(EquatableList<MarkdownElement> content, CancellationToken cancellationToken)
    {
        var links = new EquatableList<Link>();
        var pattern = LinkPattern();
        foreach (var element in content)
        {
            var textElement = element as MarkdownTextElement;
            if (textElement == null)
                continue;

            var matches = pattern.Matches(textElement.Text);
            foreach (Match match in matches.Where(m => m != null))
            {
                links.Add(new Link
                {
                    Name = match.Groups[1].Value,
                    Uri = new Uri(match.Groups[2].Value)
                });
            }
        }
        return Task.FromResult(links);
    }


    [GeneratedRegex(@"\[(.*?)\]\((.*?)\)")]
    private static partial Regex LinkPattern();
}
