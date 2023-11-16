namespace AJE.Domain.Exceptions;

[Serializable]
public class KeyExistsException : Exception
{
    public KeyExistsException(string? message)
        : base(message)
    {
    }

    protected KeyExistsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
