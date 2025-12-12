using CSharpFunctionalExtensions;
using Domain.Agency;
using Domain.Agency.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IAgencyRepository
    {
        Task<Result<AgencyEntity>> GetByIdAsync(AgencyId id);
        Task<Result> SaveAsync(AgencyEntity agencyEntity);
        Task<Result> UpdateAsync(AgencyEntity agencyEntity);
        Task<Result> DeleteAsync(AgencyId id);
    }
}