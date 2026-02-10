using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;
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
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return buyer != null
                ? Result.Success(buyer)
                : Result.Failure<BuyerEntity>($"Buyer with ID {id.Value} not found");
        }

        public async Task<Result<BuyerEntity>> GetByClientIdAsync(ClientId clientId)
        {
            var buyer = await _context.Buyers
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ClientId == clientId);

            return buyer != null
                ? Result.Success(buyer)
                : Result.Failure<BuyerEntity>($"Buyer for ClientId {clientId.Value} not found");
        }

        public async Task<Result<IEnumerable<BuyerEntity>>> GetAllAsync()
        {
            var buyers = await _context.Buyers.AsNoTracking().ToListAsync();
            return Result.Success<IEnumerable<BuyerEntity>>(buyers);
        }

        public async Task<Result<(IEnumerable<BuyerEntity> Items, int TotalCount)>> SearchAsync(
            int page,
            int pageSize)
        {
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 1 : pageSize;

            var query = _context.Buyers.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(b => b.Id.Value)
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync();

            return Result.Success((items.AsEnumerable(), totalCount));
        }

        public Result<BuyerEntity> Add(BuyerEntity buyerEntity)
        {
            _context.Buyers.Add(buyerEntity);
            return Result.Success(buyerEntity);
        }

        public Result Update(BuyerEntity buyerEntity)
        {
            _context.Buyers.Update(buyerEntity);
            return Result.Success();
        }

        public Result Delete(BuyerId id)
        {
            var buyer = _context.Buyers.FirstOrDefault(b => b.Id == id);
            if (buyer == null)
            {
                return Result.Failure($"Buyer with ID {id.Value} not found");
            }

            _context.Buyers.Remove(buyer);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(BuyerId id)
        {
            return await _context.Buyers.AsNoTracking().AnyAsync(b => b.Id == id);
        }

        public async Task<bool> ExistsByClientIdAsync(ClientId clientId)
        {
            return await _context.Buyers.AsNoTracking().AnyAsync(b => b.ClientId == clientId);
        }
    }
}
