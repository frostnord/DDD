using CSharpFunctionalExtensions;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Agency.VO;

public class AgencyId : TypedId<AgencyId>
{
    private AgencyId(Guid value) : base(value) { }

    public static Result<AgencyId> Create(Guid value)
        => TypedId<AgencyId>.Create(value, v => new AgencyId(v));
}