using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Seller;
using Domain.Domain.Customers.Seller.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext _context;

        public SellerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Seller>> GetByIdAsync(SellerId id)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Id == id);

            return seller != null
                ? Result.Success(seller)
                : Result.Failure<Seller>($"Seller with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Seller>>> GetAllAsync()
        {
            var sellers = await _context.Sellers.ToListAsync();
            return Result.Success<IEnumerable<Seller>>(sellers);
        }

        public async Task<Result<Seller>> AddAsync(Seller seller)
        {
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
            return Result.Success(seller);
        }

        public async Task<Result> UpdateAsync(Seller seller)
        {
            _context.Sellers.Update(seller);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(SellerId id)
        {
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.Id == id);
            if (seller == null)
            {
                return Result.Failure($"Seller with ID {id.Value} not found");
            }

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(SellerId id)
        {
            return await _context.Sellers.AnyAsync(s => s.Id == id);
        }
    }
}