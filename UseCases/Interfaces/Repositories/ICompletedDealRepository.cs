using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface ICompletedDealRepository
    {
        Task<Result<CompletedDealEntity>> GetByIdAsync(CompletedDealId id);
        Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(ClientId clientId);
        Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(PropertyId propertyId);
        Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync();
        Task<Result<CompletedDealEntity>> AddAsync(CompletedDealEntity dealEntity);
        Task<Result> UpdateAsync(CompletedDealEntity dealEntity);
        Task<Result> DeleteAsync(CompletedDealId id);
        Task<bool> ExistsAsync(CompletedDealId id);
    }
}
