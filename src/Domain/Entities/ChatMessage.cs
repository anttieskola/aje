namespace AJE.Domain.Entities;


public static class ChatConstants
{
    public static readonly string CHANNEL = "chat";
}

public class ChatMessage
{
    [Required]
    [StringLength(36, MinimumLength = 1, ErrorMessage = "length must be 1-36")]
    [JsonPropertyName("user")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "length must be 1-256")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
