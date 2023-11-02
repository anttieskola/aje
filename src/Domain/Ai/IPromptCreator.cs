namespace AJE.Domain.Ai;

public interface IPromptCreator
{
    string Context(string context);
    string Chat(string message);
    string Chat(string message, AiChatHistoryEntry[] history);
    string[] StopWords { get; }
}