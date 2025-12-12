using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.VO;
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

        public async Task<Result<CompletedDeal>> GetByIdAsync(CompletedDealId id)
        {
            var deal = await _context.CompletedDeals
                .FirstOrDefaultAsync(d => d.Id == id);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<CompletedDeal>($"Completed deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<CompletedDeal>>> GetByClientIdAsync(ClientId clientId)
        {
            var deals = await _context.CompletedDeals
                .Where(d => d.BuyerClientId == clientId || d.SellerClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<CompletedDeal>>(deals);
        }

        public async Task<Result<IEnumerable<CompletedDeal>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var deals = await _context.CompletedDeals
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<CompletedDeal>>(deals);
        }

        public async Task<Result<CompletedDeal>> AddAsync(CompletedDeal deal)
        {
            await _context.CompletedDeals.AddAsync(deal);
            await _context.SaveChangesAsync();
            return Result.Success(deal);
        }

        public async Task<Result> UpdateAsync(CompletedDeal deal)
        {
            _context.CompletedDeals.Update(deal);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(CompletedDealId id)
        {
            var deal = await _context.CompletedDeals.FirstOrDefaultAsync(d => d.Id == id);
            if (deal == null)
            {
                return Result.Failure($"Completed deal with ID {id.Value} not found");
            }

            _context.CompletedDeals.Remove(deal);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(CompletedDealId id)
        {
            return await _context.CompletedDeals.AnyAsync(d => d.Id == id);
        }
    }
}