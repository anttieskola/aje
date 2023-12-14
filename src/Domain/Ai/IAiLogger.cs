namespace AJE.Domain.Ai;

public interface IAiLogger
{
    Task LogAsync(string fileNamePrefix, CompletionRequest request, CompletionResponse response);
    void Log(string message);
}
