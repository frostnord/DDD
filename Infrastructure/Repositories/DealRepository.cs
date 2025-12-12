using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
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

        public async Task<Result<DealEntity>> GetByIdAsync(DealId id)
        {
            var deal = await _context.Deals
                .FirstOrDefaultAsync(d => d.Id == id);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<DealEntity>($"Deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(ClientId clientId)
        {
            var deals = await _context.Deals
                .Where(d => d.ClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<DealEntity>>(deals);
        }

        public async Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var deals = await _context.Deals
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<DealEntity>>(deals);
        }

        public async Task<Result<DealEntity>> AddAsync(DealEntity dealEntity)
        {
            await _context.Deals.AddAsync(dealEntity);
            await _context.SaveChangesAsync();
            return Result.Success(dealEntity);
        }

        public async Task<Result> UpdateAsync(DealEntity dealEntity)
        {
            _context.Deals.Update(dealEntity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(DealId id)
        {
            var deal = await _context.Deals.FirstOrDefaultAsync(d => d.Id == id);
            if (deal == null)
            {
                return Result.Failure($"Deal with ID {id.Value} not found");
            }

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(DealId id)
        {
            return await _context.Deals.AnyAsync(d => d.Id == id);
        }
    }
}