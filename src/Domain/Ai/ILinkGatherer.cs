namespace AJE.Domain;

/// <summary>
/// Task might be useless but lets leave it if we want to do something async in the future
/// </summary>
public interface ILinkGatherer
{
    Task<EquatableList<Link>> GetLinksAsync(EquatableList<MarkdownElement> content, CancellationToken cancellationToken);
}
