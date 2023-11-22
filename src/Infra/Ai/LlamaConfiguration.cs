namespace AJE.Infra.Ai;

public class LlamaConfiguration
{
    public required string Host { get; set; }
    public required string LogFolder { get; set; }
    public int TimeoutInSeconds { get; set; } = 30;
    public int MaxTokenCount { get; set; } = 2048;
}
