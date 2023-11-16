namespace AJE.Domain.Ai;

public interface IContextCreator<in T> where T : class
{
    string Create(T from);
}
