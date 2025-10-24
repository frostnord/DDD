using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Buyer;
using Domain.Domain.Customers.Buyer.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IBuyerRepository
    {
        Task<Result<Buyer>> GetByIdAsync(BuyerId id);
        Task<Result<IEnumerable<Buyer>>> GetAllAsync();
        Task<Result<Buyer>> AddAsync(Buyer buyer);
        Task<Result> UpdateAsync(Buyer buyer);
        Task<Result> DeleteAsync(BuyerId id);
        Task<bool> ExistsAsync(BuyerId id);
    }
}