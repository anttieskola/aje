namespace AJE.Infra;

public class LlamaConfiguration
{
    public required string Host { get; set; }
    public required string LogFolder { get; set; }
    public int TimeoutInSeconds { get; set; } = 30;
}
