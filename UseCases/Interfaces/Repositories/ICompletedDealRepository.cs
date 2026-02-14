using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;

namespace UseCases.Interfaces.Repositories;

public interface ICompletedDealRepository
{
    Task<Result<CompletedDealEntity>> GetByIdAsync(CompletedDealId id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(PropertyId propertyId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Result<CompletedDealEntity> Add(CompletedDealEntity dealEntity);
    Result Update(CompletedDealEntity dealEntity);
    Result Delete(CompletedDealId id);
    Task<bool> ExistsAsync(CompletedDealId id, CancellationToken cancellationToken = default);
}
