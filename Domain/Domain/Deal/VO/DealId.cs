using CSharpFunctionalExtensions;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Deal;

public class DealId : TypedId<DealId>
{
    private DealId(Guid value) : base(value) { }

    public static Result<DealId> Create(Guid value)
        => TypedId<DealId>.Create(value, v => new DealId(v));
}