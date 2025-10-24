using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Customers.Seller;
using Domain.Domain.Customers.Seller.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Interfaces.Repositories
{
    public interface ISellerRepository
    {
        Task<Result<Seller>> GetByIdAsync(SellerId id);
        Task<Result<IEnumerable<Seller>>> GetAllAsync();
        Task<Result<Seller>> AddAsync(Seller seller);
        Task<Result> UpdateAsync(Seller seller);
        Task<Result> DeleteAsync(SellerId id);
        Task<bool> ExistsAsync(SellerId id);
    }
}