using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
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

        public async Task<Result<SellerEntity>> GetByIdAsync(SellerId id)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Id == id);

            return seller != null
                ? Result.Success(seller)
                : Result.Failure<SellerEntity>($"Seller with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<SellerEntity>>> GetAllAsync()
        {
            var sellers = await _context.Sellers.ToListAsync();
            return Result.Success<IEnumerable<SellerEntity>>(sellers);
        }

        public async Task<Result<SellerEntity>> AddAsync(SellerEntity sellerEntity)
        {
            await _context.Sellers.AddAsync(sellerEntity);
            await _context.SaveChangesAsync();
            return Result.Success(sellerEntity);
        }

        public async Task<Result> UpdateAsync(SellerEntity sellerEntity)
        {
            _context.Sellers.Update(sellerEntity);
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