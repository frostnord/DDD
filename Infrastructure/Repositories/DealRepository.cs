using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.VO;
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

        public async Task<Result<Deal>> GetByIdAsync(DealId id)
        {
            var deal = await _context.Deals
                .FirstOrDefaultAsync(d => d.Id == id);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<Deal>($"Deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Deal>>> GetByClientIdAsync(ClientId clientId)
        {
            var deals = await _context.Deals
                .Where(d => d.ClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<Deal>>(deals);
        }

        public async Task<Result<IEnumerable<Deal>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var deals = await _context.Deals
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<Deal>>(deals);
        }

        public async Task<Result<Deal>> AddAsync(Deal deal)
        {
            await _context.Deals.AddAsync(deal);
            await _context.SaveChangesAsync();
            return Result.Success(deal);
        }

        public async Task<Result> UpdateAsync(Deal deal)
        {
            _context.Deals.Update(deal);
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