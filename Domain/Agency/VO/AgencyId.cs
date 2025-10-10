using CSharpFunctionalExtensions;


namespace DDD.Domain.ValueObjects.AgencyVO;

public class AgencyId : TypedId<AgencyId>
{
    private AgencyId(Guid value) : base(value) { }

    public static Result<AgencyId> Create(Guid value)
        => TypedId<AgencyId>.Create(value, v => new AgencyId(v));

    public static AgencyId New() => new(Guid.NewGuid());
}