namespace AJE.Domain.Entities;

public record ResourceRequest
{
    public required Guid RequestId { get; set; }
    public required string ResourceName { get; set; }
    public required DateTimeOffset Time { get; set; }
}