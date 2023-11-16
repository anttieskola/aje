

namespace AJE.Domain.Exceptions;

[Serializable]
public class AiException : Exception
{
    public AiException(string? message)
        : base(message)
    {
    }

    protected AiException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}