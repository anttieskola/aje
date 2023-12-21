namespace AJE.Infra.Translate;

public class TranslateConfiguration
{
    public required string Host { get; set; }
    public int TimeoutInSeconds { get; set; } = 30;
}
