
namespace AJE.Domain.Queries;

public static class GuidCharacter
{
    public static bool IsValid(char c)
    {
        if (char.IsDigit(c))
            return true;

        if (c == 'a' || c == 'A'
            || c == 'b' || c == 'B'
            || c == 'c' || c == 'C'
            || c == 'd' || c == 'D'
            || c == 'e' || c == 'E'
            || c == 'f' || c == 'F')
            return true;
        return false;
    }
}

/// <summary>
///
/// </summary>
public record GuidGetQuery : IRequest<Guid>, IValidatableObject
{
    /// <summary>
    /// Must be 8 characters long, valid characters are 0-9 and a-f
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Can be any length and contain 8 valid characters that are 0-9 and a-f
    /// </summary>
    public required string UniqueString { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Category))
            yield return new ValidationResult("Category is required", new[] { nameof(Category) });
        else
        {
            if (Category.Length != 8)
                yield return new ValidationResult("Category must be 8 characters long", new[] { nameof(Category) });

            foreach (var c in Category)
            {
                if (!GuidCharacter.IsValid(c))
                    yield return new ValidationResult("Category contains invalid characters", new[] { nameof(Category) });
            }
        }
        if (string.IsNullOrWhiteSpace(UniqueString))
            yield return new ValidationResult("UniqueString is required", new[] { nameof(UniqueString) });
        else
        {
            var validCharacters = 0;
            foreach (var c in UniqueString)
            {
                if (GuidCharacter.IsValid(c))
                    validCharacters++;
            }
            if (validCharacters < 8)
                yield return new ValidationResult("UniqueString must contain atleast 8 valid characters", new[] { nameof(UniqueString) });
        }
    }
}

public class GuidGetQueryHandler : IRequestHandler<GuidGetQuery, Guid>
{
    public Task<Guid> Handle(GuidGetQuery request, CancellationToken cancellationToken)
    {
        Validator.ValidateObject(request, new ValidationContext(request), true);
        var sb = new StringBuilder();
        sb.Append(request.Category);
        sb.Append('-');
        foreach (var c in request.UniqueString)
        {
            if (GuidCharacter.IsValid(c))
                sb.Append(c);
            if (sb.Length == 13 || sb.Length == 18 || sb.Length == 23)
                sb.Append('-');
            if (sb.Length == 32)
                break;
        }
        while (sb.Length < 36)
        {
            if (sb.Length == 18 || sb.Length == 23)
                sb.Append('-');
            else
                sb.Append('0');
        }
        return Task.FromResult(Guid.ParseExact(sb.ToString(), "D"));
    }
}
