namespace AJE.Infra.AiRedis;

public class LlamaServer
{
    public required string ResourceName { get; set; }
    public required string Host { get; set; }
    public int MaxTokenCount { get; set; } = 16384;
    public int TimeoutInSeconds { get; set; } = 3600;
}

public class LlamaConfiguration
{
    public required string LogFolder { get; set; }
    public required LlamaServer[] Servers { get; set; }
}
