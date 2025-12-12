using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Deal;

public class CompletedDealId : TypedId<CompletedDealId>
{
    private CompletedDealId(Guid value) : base(value)
    {
    }

    public static Result<CompletedDealId> Create(Guid value)
        => Create(value, v => new CompletedDealId(v));
}