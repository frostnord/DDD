using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;

namespace UseCases.Interfaces.Repositories;

public interface IBuyerRepository
{
    Task<Result<BuyerEntity>> GetByIdAsync(BuyerId id);
    Task<Result<IEnumerable<BuyerEntity>>> GetAllAsync();
    Task<Result<(IEnumerable<BuyerEntity> Items, int TotalCount)>> SearchAsync(int page, int pageSize);
    Task<Result<BuyerEntity>> AddAsync(BuyerEntity buyerEntity);
    Task<Result> UpdateAsync(BuyerEntity buyerEntity);
    Task<Result> DeleteAsync(BuyerId id);
    Task<bool> ExistsAsync(BuyerId id);
}
