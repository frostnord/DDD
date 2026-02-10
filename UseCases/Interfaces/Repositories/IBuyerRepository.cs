using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories;

public interface IBuyerRepository
{
    Task<Result<BuyerEntity>> GetByIdAsync(BuyerId id);
    Task<Result<IEnumerable<BuyerEntity>>> GetAllAsync();
    Task<Result<(IEnumerable<BuyerEntity> Items, int TotalCount)>> SearchAsync(int page, int pageSize);
    Result<BuyerEntity> Add(BuyerEntity buyerEntity);
    Result Update(BuyerEntity buyerEntity);
    Result Delete(BuyerId id);
    Task<bool> ExistsAsync(BuyerId id);
    Task<bool> ExistsByClientIdAsync(ClientId clientId);
}
