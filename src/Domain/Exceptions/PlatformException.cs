namespace AJE.Domain.Exceptions;

[Serializable]
public class PlatformException : Exception
{
    public PlatformException(string? message)
        : base(message)
    {
    }

    protected PlatformException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
