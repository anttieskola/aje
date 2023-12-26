namespace AJE.Domain.Exceptions;

public class AiParseException : Exception
{
    public AiParseException(string? message)
        : base(message)
    {
    }
}