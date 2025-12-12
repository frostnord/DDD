using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly AppDbContext _context;

        public BuyerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<BuyerEntity>> GetByIdAsync(BuyerId id)
        {
            var buyer = await _context.Buyers
                .FirstOrDefaultAsync(b => b.Id == id);

            return buyer != null
                ? Result.Success(buyer)
                : Result.Failure<BuyerEntity>($"Buyer with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<BuyerEntity>>> GetAllAsync()
        {
            var buyers = await _context.Buyers.ToListAsync();
            return Result.Success<IEnumerable<BuyerEntity>>(buyers);
        }

        public async Task<Result<BuyerEntity>> AddAsync(BuyerEntity buyerEntity)
        {
            await _context.Buyers.AddAsync(buyerEntity);
            await _context.SaveChangesAsync();
            return Result.Success(buyerEntity);
        }

        public async Task<Result> UpdateAsync(BuyerEntity buyerEntity)
        {
            _context.Buyers.Update(buyerEntity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(BuyerId id)
        {
            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Id == id);
            if (buyer == null)
            {
                return Result.Failure($"Buyer with ID {id.Value} not found");
            }

            _context.Buyers.Remove(buyer);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(BuyerId id)
        {
            return await _context.Buyers.AnyAsync(b => b.Id == id);
        }
    }
}