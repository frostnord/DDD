using CSharpFunctionalExtensions;
using Domain.Deal;

namespace UseCases.Interfaces.Services;

public interface ICompletedDealService
{
    Task<Result<CompletedDealEntity>> CreateAsync(Guid buyerClientId, Guid sellerClientId, Guid propertyId,
        DateTime dealDate, decimal dealAmount, string dealType);

    Task<Result<CompletedDealEntity>> GetByIdAsync(Guid completedDealId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(Guid clientId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(Guid propertyId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync();

    Task<Result> DeleteAsync(Guid completedDealId);
}
