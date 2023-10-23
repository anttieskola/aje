
namespace AJE.Domain.Ai;

public interface IAiModel
{
    Task<CompletionResponse> CompletionAsync(CompletionRequest request, CancellationToken cancellationToken);
}
