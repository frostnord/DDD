using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Buyer;
using Domain.Domain.Customers.Buyer.VO;
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

        public async Task<Result<Buyer>> GetByIdAsync(BuyerId id)
        {
            var buyer = await _context.Buyers
                .FirstOrDefaultAsync(b => b.Id == id);

            return buyer != null
                ? Result.Success(buyer)
                : Result.Failure<Buyer>($"Buyer with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Buyer>>> GetAllAsync()
        {
            var buyers = await _context.Buyers.ToListAsync();
            return Result.Success<IEnumerable<Buyer>>(buyers);
        }

        public async Task<Result<Buyer>> AddAsync(Buyer buyer)
        {
            await _context.Buyers.AddAsync(buyer);
            await _context.SaveChangesAsync();
            return Result.Success(buyer);
        }

        public async Task<Result> UpdateAsync(Buyer buyer)
        {
            _context.Buyers.Update(buyer);
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