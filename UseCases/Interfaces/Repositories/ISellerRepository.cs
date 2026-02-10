using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;

namespace UseCases.Interfaces.Repositories;

public interface ISellerRepository
{
    Task<Result<SellerEntity>> GetByIdAsync(SellerId id);
    Task<Result<IEnumerable<SellerEntity>>> GetAllAsync();
    Result<SellerEntity> Add(SellerEntity sellerEntity);
    Result<SellerEntity> Update(SellerEntity sellerEntity);
    Result Delete(SellerId id);
    Task<bool> ExistsAsync(SellerId id);
    Task<bool> ExistsByClientIdAsync(ClientId clientId);
}
