using CSharpFunctionalExtensions;
using Domain.Deal;

namespace UseCases.Interfaces.Services
{
    public interface IDealService
    {
        Task<Result<DealEntity>> CreateDealAsync(Guid propertyId, Guid clientId, decimal price, DateTime dealDate);
        Task<Result<DealEntity>> GetDealByIdAsync(Guid dealId);
        Task<Result<IEnumerable<DealEntity>>> GetAllDealsAsync();
        Task<Result> UpdateDealAsync(Guid dealId, Guid propertyId, Guid clientId, decimal price, DateTime dealDate);
        Task<Result> DeleteDealAsync(Guid dealId);
    }
}