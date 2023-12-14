
namespace AJE.Infra.Ai;

public class AiLogger : IAiLogger
{
    private readonly ILogger<AiLogger> _logger;
    private readonly string _logFolder;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public AiLogger(
        ILogger<AiLogger> logger,
        LlamaConfiguration configuration)
    {
        _logger = logger;
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
            JsonSerializer.Serialize(request, _jsonSerializerOptions));

        await File.WriteAllTextAsync(
            Path.Combine(_logFolder, $"{fileNamePrefix}-response.json"),
            JsonSerializer.Serialize(response, _jsonSerializerOptions));
    }

    public void Log(string message)
    {
        _logger.LogWarning(message);
    }
}
