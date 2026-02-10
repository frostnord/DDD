using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Deal.VO;

public class DealId : TypedId<DealId>
{
    private DealId(Guid value) : base(value)
    {
    }

    public static Result<DealId> Create(Guid value)
        => Create(value, v => new DealId(v));
}