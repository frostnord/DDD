using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories;

public interface IDealRepository
{
    Task<Result<DealEntity>> GetByIdAsync(DealId id);
    Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(ClientId clientId);
    Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(PropertyId propertyId);
    Task<Result<IEnumerable<DealEntity>>> GetAllAsync();
    Result<DealEntity> Add(DealEntity dealEntity);
    Result Update(DealEntity dealEntity);
    Result Delete(DealId id);
    Task<bool> ExistsAsync(DealId id);
}
