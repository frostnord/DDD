using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories;

public interface IBuyerRepository
{
    Task<Result<BuyerEntity>> GetByIdAsync(BuyerId id, CancellationToken cancellationToken = default);
    Task<Result<BuyerEntity>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<BuyerEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<(IEnumerable<BuyerEntity> Items, int TotalCount)>> SearchAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Result<BuyerEntity> Add(BuyerEntity buyerEntity);
    Result Update(BuyerEntity buyerEntity);
    Result Delete(BuyerId id);
    Task<bool> ExistsAsync(BuyerId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
}
