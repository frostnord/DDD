using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Seller;

namespace UseCases.Interfaces
{
    public interface ISellerService
    {
        Task<Result<Seller>> CreateSellerAsync(Guid clientId);
        Task<Result<Seller>> GetSellerByIdAsync(Guid sellerId);
        Task<Result<IEnumerable<Seller>>> GetAllSellersAsync();
        Task<Result> UpdateSellerAsync(Guid sellerId, Guid clientId);
        Task<Result> DeleteSellerAsync(Guid sellerId);
    }
}