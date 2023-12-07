namespace AJE.Domain.Exceptions;

public class PlatformException : Exception
{
    public PlatformException(string? message)
        : base(message)
    {
    }
}
