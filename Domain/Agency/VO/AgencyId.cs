using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Agency.VO;

public class AgencyId : TypedId<AgencyId>
{
    private AgencyId(Guid value) : base(value)
    {
    }

    public static Result<AgencyId> Create(Guid value)
        => Create(value, v => new AgencyId(v));
}