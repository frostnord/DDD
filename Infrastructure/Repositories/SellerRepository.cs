using System.Collections.Generic;
using System.Linq;
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

        public async Task<Result<(IEnumerable<SellerEntity> Items, int TotalCount)>> SearchAsync(
            int page,
            int pageSize,
            string sortBy,
            string sortOrder,
            CancellationToken cancellationToken = default)
        {
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 1 : pageSize;

            var normalizedSortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim();
            var normalizedSortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder.Trim();

            var query = _context.Sellers.AsNoTracking().AsQueryable();

            var isDesc = string.Equals(normalizedSortOrder, "desc", System.StringComparison.OrdinalIgnoreCase);

            query = normalizedSortBy.ToLowerInvariant() switch
            {
                "id" => isDesc ? query.OrderByDescending(s => s.Id.Value) : query.OrderBy(s => s.Id.Value),
                "clientid" => isDesc ? query.OrderByDescending(s => s.ClientId.Value) : query.OrderBy(s => s.ClientId.Value),
                "registeredat" => isDesc ? query.OrderByDescending(s => s.RegisteredAt) : query.OrderBy(s => s.RegisteredAt),
                _ => query.OrderBy(s => s.Id.Value)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

            return Result.Success((items.AsEnumerable(), totalCount));
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
