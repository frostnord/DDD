using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Property.VO;

public class PropertyId : TypedId<PropertyId>
{
    private PropertyId(Guid value) : base(value)
    {
    }

    public static Result<PropertyId> Create(Guid value)
        => Create(value, v => new PropertyId(v));
}