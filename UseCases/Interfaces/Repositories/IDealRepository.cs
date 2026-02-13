using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories;

public interface IDealRepository
{
    Task<Result<DealEntity>> GetByIdAsync(DealId id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(PropertyId propertyId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DealEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Result<DealEntity> Add(DealEntity dealEntity);
    Result Update(DealEntity dealEntity);
    Result Delete(DealId id);
    Task<bool> ExistsAsync(DealId id, CancellationToken cancellationToken = default);
}
