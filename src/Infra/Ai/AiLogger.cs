
namespace AJE.Infra.Ai;

public class AiLogger : IAiLogger
{
    private readonly string _logFolder;

    public AiLogger(LlamaConfiguration configuration)
    {
        _logFolder = configuration.LogFolder;

        if (!Directory.Exists(_logFolder))
            Directory.CreateDirectory(_logFolder);

        var testFile = Path.Combine(_logFolder, "test.txt");
        File.WriteAllText(testFile, "test");
        File.Delete(testFile);
    }

    public async Task LogAsync(string fileNamePrefix, CompletionRequest request, CompletionResponse response)
    {
        await File.WriteAllTextAsync(
            Path.Combine(_logFolder, $"{fileNamePrefix}-request.json"),
            JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }));

        await File.WriteAllTextAsync(
            Path.Combine(_logFolder, $"{fileNamePrefix}-response.json"),
            JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }
}
