using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class CompletedDealRepository : ICompletedDealRepository
    {
        private readonly AppDbContext _context;

        public CompletedDealRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CompletedDealEntity>> GetByIdAsync(CompletedDealId id, CancellationToken cancellationToken = default)
        {
            var deal = await _context.CompletedDeals
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<CompletedDealEntity>($"Completed deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default)
        {
            var deals = await _context.CompletedDeals
                .AsNoTracking()
                .Where(d => d.BuyerClientId == clientId || d.SellerClientId == clientId)
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(PropertyId propertyId, CancellationToken cancellationToken = default)
        {
            var deals = await _context.CompletedDeals
                .AsNoTracking()
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }

        public Result<CompletedDealEntity> Add(CompletedDealEntity dealEntity)
        {
            _context.CompletedDeals.Add(dealEntity);
            return Result.Success(dealEntity);
        }

        public Result Update(CompletedDealEntity dealEntity)
        {
            _context.CompletedDeals.Update(dealEntity);
            return Result.Success();
        }

        public Result Delete(CompletedDealId id)
        {
            var deal = _context.CompletedDeals.FirstOrDefault(d => d.Id == id);
            if (deal == null)
            {
                return Result.Failure($"Completed deal with ID {id.Value} not found");
            }

            _context.CompletedDeals.Remove(deal);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(CompletedDealId id, CancellationToken cancellationToken = default)
        {
            return await _context.CompletedDeals.AsNoTracking().AnyAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var deals = await _context.CompletedDeals.AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }
    }
}
