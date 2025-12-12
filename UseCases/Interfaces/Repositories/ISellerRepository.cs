using CSharpFunctionalExtensions;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface ISellerRepository
    {
        Task<Result<SellerEntity>> GetByIdAsync(SellerId id);
        Task<Result<IEnumerable<SellerEntity>>> GetAllAsync();
        Task<Result<SellerEntity>> AddAsync(SellerEntity sellerEntity);
        Task<Result> UpdateAsync(SellerEntity sellerEntity);
        Task<Result> DeleteAsync(SellerId id);
        Task<bool> ExistsAsync(SellerId id);
    }
}