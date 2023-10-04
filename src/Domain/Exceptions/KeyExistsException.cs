namespace AJE.Domain.Exceptions;

public class KeyExistsException : Exception
{
    public KeyExistsException(string? message) : base(message)
    {
    }
}
