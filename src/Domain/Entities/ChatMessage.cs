namespace AJE.Domain.Entities;

public class ChatMessage
{
    [JsonPropertyName("user")]
    public string User { get; set; } = string.Empty;
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
