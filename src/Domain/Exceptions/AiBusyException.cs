namespace AJE.Domain.Exceptions;

public class AiBusyException : Exception
{
    public AiBusyException(string? message)
        : base(message)
    {
    }
}