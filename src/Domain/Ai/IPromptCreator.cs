namespace AJE.Domain.Ai;

public interface IPromptCreator
{
    string Create(string context);

    string[] StopWords { get; }
}