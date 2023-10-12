namespace AJE.Domain.Exceptions;

[Serializable]
public class KeyExistsException : Exception
{
    public KeyExistsException(string? message) : base(message)
    {
    }
}
