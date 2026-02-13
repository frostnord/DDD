using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
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

        public async Task<Result<SellerEntity>> GetByIdAsync(SellerId id, CancellationToken cancellationToken = default)
        {
            var seller = await _context.Sellers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            return seller != null
                ? Result.Success(seller)
                : Result.Failure<SellerEntity>($"Seller with ID {id.Value} not found");
        }

        public async Task<Result<SellerEntity>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default)
        {
            var seller = await _context.Sellers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ClientId == clientId, cancellationToken);

            return seller != null
                ? Result.Success(seller)
                : Result.Failure<SellerEntity>($"Seller for ClientId {clientId.Value} not found");
        }

        public async Task<Result<IEnumerable<SellerEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sellers = await _context.Sellers.AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<SellerEntity>>(sellers);
        }

        public Result<SellerEntity> Add(SellerEntity sellerEntity)
        {
            _context.Sellers.Add(sellerEntity);
            return Result.Success(sellerEntity);
        }

        public Result<SellerEntity> Update(SellerEntity sellerEntity)
        {
            _context.Sellers.Update(sellerEntity);
            return Result.Success(sellerEntity);
        }

        public Result Delete(SellerId id)
        {
            var seller = _context.Sellers.FirstOrDefault(s => s.Id == id);
            if (seller == null)
            {
                return Result.Failure($"Seller with ID {id.Value} not found");
            }

            _context.Sellers.Remove(seller);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(SellerId id, CancellationToken cancellationToken = default)
        {
            return await _context.Sellers.AsNoTracking().AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default)
        {
            return await _context.Sellers.AsNoTracking().AnyAsync(s => s.ClientId == clientId, cancellationToken);
        }
    }
}
