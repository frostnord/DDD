using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;

namespace DDD.Domain.ValueObjects;

public class PropertyId : TypedId<PropertyId>
{
    private PropertyId(Guid value) : base(value) { }

    public static Result<PropertyId> Create(Guid value)
        => TypedId<PropertyId>.Create(value, v => new PropertyId(v));

    public static PropertyId New() => new (Guid.NewGuid());
}