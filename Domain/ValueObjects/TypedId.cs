using CSharpFunctionalExtensions;

namespace DDD.Domain;

public abstract class TypedId<T> : ValueObject, IComparable<T>, IComparable
    where T : TypedId<T>
{
    public Guid Value { get; }

    protected TypedId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ID не может быть пустым", nameof(value));

        Value = value;
    }

    public int CompareTo(T other)
    {
        if (other == null) return 1;
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object obj)
    {
        if (obj is T other)
            return CompareTo(other);
        if (obj is null) return 1;
        throw new ArgumentException($"Object is not of type {typeof(T).Name}");
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static Result<T> Create(Guid value, Func<Guid, T> factory)
    {
        if (value == Guid.Empty)
            return Result.Failure<T>("ID не может быть пустым");

        return Result.Success(factory(value));
    }
}