namespace AJE.Domain.Exceptions;

[Serializable]
public class SubscriptionException : Exception
{
    public SubscriptionException(string? message)
        : base(message)
    {
    }

    protected SubscriptionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
