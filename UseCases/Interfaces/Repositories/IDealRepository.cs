using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IDealRepository
    {
        Task<Result<Deal>> GetByIdAsync(DealId id);
        Task<Result<IEnumerable<Deal>>> GetByClientIdAsync(ClientId clientId);
        Task<Result<IEnumerable<Deal>>> GetByPropertyIdAsync(PropertyId propertyId);
        Task<Result<Deal>> AddAsync(Deal deal);
        Task<Result> UpdateAsync(Deal deal);
        Task<Result> DeleteAsync(DealId id);
        Task<bool> ExistsAsync(DealId id);
    }
}