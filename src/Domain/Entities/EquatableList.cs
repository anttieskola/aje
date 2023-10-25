namespace AJE.Domain.Entities;

/// <summary>
/// Collection type for used in records to allow for equality comparison
/// </summary>
/// <typeparam name="T"></typeparam>
public class EquatableList<T> : List<T>, IEquatable<EquatableList<T>>
{
    public static EquatableList<T> Empty => new();

    public bool Equals(EquatableList<T>? other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        for (int i = 0; i < Count; i++)
        {
            var a = this[i];
            var b = other[i];

            if (a == null || b == null)
                return false;

            if (!a.Equals(b))
                return false;
        }
        return true;
    }

    public override bool Equals([AllowNull] object obj)
    {
        return Equals(obj as EquatableList<T>);
    }

    public override int GetHashCode()
    {
        var hash = 3;
        foreach (var item in this)
            hash = item != null ? hash * 5 + item.GetHashCode() : hash * 7;
        return hash;
    }
}
