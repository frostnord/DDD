using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories;

public interface ICompletedDealRepository
{
    Task<Result<CompletedDealEntity>> GetByIdAsync(CompletedDealId id);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(ClientId clientId);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(PropertyId propertyId);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync();
    Result<CompletedDealEntity> Add(CompletedDealEntity dealEntity);
    Result Update(CompletedDealEntity dealEntity);
    Result Delete(CompletedDealId id);
    Task<bool> ExistsAsync(CompletedDealId id);
}
