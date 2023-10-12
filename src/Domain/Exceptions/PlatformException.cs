namespace AJE.Domain.Exceptions;

[Serializable]
public class PlatformException : Exception
{
    public PlatformException(string? message) : base(message)
    {
    }
}
