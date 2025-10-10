using System;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects.CommonVO;

namespace DDD.Domain.ValueObjects;

public class CompletedDealId : TypedId<CompletedDealId>
{
    private CompletedDealId(Guid value) : base(value) { }

    public static Result<CompletedDealId> Create(Guid value)
        => TypedId<CompletedDealId>.Create(value, v => new CompletedDealId(v));

    public static CompletedDealId New() => new(Guid.NewGuid());
}
