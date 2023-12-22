namespace AJE.Domain.Ai;

public partial class YlePersonGatherer : IPersonGatherer
{
    public Task<EquatableList<Person>> GetPersonsAsync(EquatableList<MarkdownElement> content, CancellationToken cancellationToken)
    {
        var persons = new EquatableList<Person>();
        var pattern = PersonPattern();
        foreach (var element in content)
        {
            var textElement = element as MarkdownTextElement;
            if (textElement == null)
                continue;

            var matches = pattern.Matches(textElement.Text);
            foreach (Match match in matches.Where(m => m != null))
            {
                persons.Add(new Person
                {
                    Name = match.Groups[1].Value,
                });
            }
        }
        return Task.FromResult(persons);
    }

    [GeneratedRegex(@"\*\*(.*?)\*\*")]
    private static partial Regex PersonPattern();
}
