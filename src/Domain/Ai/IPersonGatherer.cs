namespace AJE.Domain.Ai;

public interface IPersonGatherer
{
    /// <summary>
    /// Tasks cause might take on other news sources and need AI then
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<EquatableList<Person>> GetPersonsAsync(EquatableList<MarkdownElement> content, CancellationToken cancellationToken);
}
