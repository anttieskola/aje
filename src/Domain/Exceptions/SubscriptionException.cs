namespace AJE.Domain.Exceptions;

public class SubscriptionException : Exception
{
    public SubscriptionException(string? message)
        : base(message)
    {
    }
}
