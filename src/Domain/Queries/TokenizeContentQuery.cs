
namespace AJE.Domain.Queries;

public record TokenizeContentQuery : IRequest<TokenizeResponse>
{
    public required string Content { get; init; }
}

public class TokenizeContentQueryHandler : IRequestHandler<TokenizeContentQuery, TokenizeResponse>
{
    private readonly IAiModel _aiModel;

    public TokenizeContentQueryHandler(
        IAiModel aiModel)
    {
        _aiModel = aiModel;
    }

    public Task<TokenizeResponse> Handle(TokenizeContentQuery request, CancellationToken cancellationToken)
    {
        return _aiModel.TokenizeAsync(new TokenizeRequest
        {
            Content = request.Content
        }, cancellationToken);
    }
}
