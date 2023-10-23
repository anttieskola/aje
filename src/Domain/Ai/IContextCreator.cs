namespace AJE.Domain.Ai;

public interface IContextCreator<T>
{
    string Create(T from);
}
