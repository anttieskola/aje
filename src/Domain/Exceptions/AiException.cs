namespace AJE.Domain.Exceptions;

public class AiException : Exception
{
    public AiException(string? message)
        : base(message)
    {
    }
}