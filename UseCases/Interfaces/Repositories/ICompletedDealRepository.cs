using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface ICompletedDealRepository
    {
        Task<Result<CompletedDeal>> GetByIdAsync(CompletedDealId id);
        Task<Result<IEnumerable<CompletedDeal>>> GetByClientIdAsync(ClientId clientId);
        Task<Result<IEnumerable<CompletedDeal>>> GetByPropertyIdAsync(PropertyId propertyId);
        Task<Result<CompletedDeal>> AddAsync(CompletedDeal deal);
        Task<Result> UpdateAsync(CompletedDeal deal);
        Task<Result> DeleteAsync(CompletedDealId id);
        Task<bool> ExistsAsync(CompletedDealId id);
    }
}