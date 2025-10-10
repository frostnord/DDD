using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects.CommonVO;

namespace DDD.Domain.ValueObjects.DealVO;

public class DealId : TypedId<DealId>
{
    private DealId(Guid value) : base(value) { }

    public static Result<DealId> Create(Guid value)
        => TypedId<DealId>.Create(value, v => new DealId(v));

    public static DealId New() => new(Guid.NewGuid());
}