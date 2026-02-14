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
    public class DealRepository : IDealRepository
    {
        private readonly AppDbContext _context;

        public DealRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DealEntity>> GetByIdAsync(DealId id, CancellationToken cancellationToken = default)
        {
            var deal = await _context.Deals
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<DealEntity>($"Deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(ClientId clientId, CancellationToken cancellationToken = default)
        {
            var deals = await _context.Deals
                .AsNoTracking()
                .Where(d => d.ClientId == clientId)
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<DealEntity>>(deals);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(PropertyId propertyId, CancellationToken cancellationToken = default)
        {
            var deals = await _context.Deals
                .AsNoTracking()
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<DealEntity>>(deals);
        }

        public Result<DealEntity> Add(DealEntity dealEntity)
        {
            _context.Deals.Add(dealEntity);
            return Result.Success(dealEntity);
        }

        public Result Update(DealEntity dealEntity)
        {
            _context.Deals.Update(dealEntity);
            return Result.Success();
        }

        public Result Delete(DealId id)
        {
            var deal = _context.Deals.FirstOrDefault(d => d.Id == id);
            if (deal == null)
            {
                return Result.Failure($"Deal with ID {id.Value} not found");
            }

            _context.Deals.Remove(deal);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(DealId id, CancellationToken cancellationToken = default)
        {
            return await _context.Deals.AsNoTracking().AnyAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var deals = await _context.Deals.AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<DealEntity>>(deals);
        }
    }
}
