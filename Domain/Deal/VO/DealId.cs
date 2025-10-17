using CSharpFunctionalExtensions;
using DDD.Domain;


namespace DDD.Domain.ValueObjects;

public class DealId : TypedId<DealId>
{
    private DealId(Guid value) : base(value) { }

    public static Result<DealId> Create(Guid value)
        => TypedId<DealId>.Create(value, v => new DealId(v));
}