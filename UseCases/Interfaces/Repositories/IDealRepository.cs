using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IDealRepository
    {
        Task<Result<DealEntity>> GetByIdAsync(DealId id);
        Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(ClientId clientId);
        Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(PropertyId propertyId);
        Task<Result<IEnumerable<DealEntity>>> GetAllAsync();
        Task<Result<DealEntity>> AddAsync(DealEntity dealEntity);
        Task<Result> UpdateAsync(DealEntity dealEntity);
        Task<Result> DeleteAsync(DealId id);
        Task<bool> ExistsAsync(DealId id);
    }
}