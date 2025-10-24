using CSharpFunctionalExtensions;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Deal;

public class CompletedDealId : TypedId<CompletedDealId>
{
    private CompletedDealId(Guid value) : base(value) { }

    public static Result<CompletedDealId> Create(Guid value)
        => TypedId<CompletedDealId>.Create(value, v => new CompletedDealId(v));
}
