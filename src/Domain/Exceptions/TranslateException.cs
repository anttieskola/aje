namespace AJE.Domain.Exceptions;

public class TranslateException : Exception
{
    public TranslateException(string? message)
        : base(message)
    {
    }
}