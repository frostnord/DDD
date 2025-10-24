using System;
using CSharpFunctionalExtensions;


namespace Domain.Domain.ValueObjects;

public class CompletedDealId : TypedId<CompletedDealId>
{
    private CompletedDealId(Guid value) : base(value) { }

    public static Result<CompletedDealId> Create(Guid value)
        => TypedId<CompletedDealId>.Create(value, v => new CompletedDealId(v));
}
