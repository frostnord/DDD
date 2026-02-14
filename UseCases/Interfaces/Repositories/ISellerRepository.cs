using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;

namespace UseCases.Interfaces.Repositories;

public interface ISellerRepository
{
    Task<Result<SellerEntity>> GetByIdAsync(SellerId id, CancellationToken cancellationToken = default);
    Task<Result<SellerEntity>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SellerEntity>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<(IEnumerable<SellerEntity> Items, int TotalCount)>> SearchAsync(
        int page,
        int pageSize,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken = default);

    Result<SellerEntity> Add(SellerEntity sellerEntity);
    Result<SellerEntity> Update(SellerEntity sellerEntity);
    Result Delete(SellerId id);
    Task<bool> ExistsAsync(SellerId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default);
}
