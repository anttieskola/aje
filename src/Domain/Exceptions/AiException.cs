namespace AJE.Domain.Exceptions;

[Serializable]
public class AiException : Exception
{
    public AiException(string? message) : base(message)
    {
    }
}