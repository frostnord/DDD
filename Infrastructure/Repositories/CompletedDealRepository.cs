using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
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

        public async Task<Result<CompletedDealEntity>> GetByIdAsync(CompletedDealId id)
        {
            var deal = await _context.CompletedDeals
                .FirstOrDefaultAsync(d => d.Id == id);

            return deal != null
                ? Result.Success(deal)
                : Result.Failure<CompletedDealEntity>($"Completed deal with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(ClientId clientId)
        {
            var deals = await _context.CompletedDeals
                .Where(d => d.BuyerClientId == clientId || d.SellerClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var deals = await _context.CompletedDeals
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }

        public async Task<Result<CompletedDealEntity>> AddAsync(CompletedDealEntity dealEntity)
        {
            await _context.CompletedDeals.AddAsync(dealEntity);
            await _context.SaveChangesAsync();
            return Result.Success(dealEntity);
        }

        public async Task<Result> UpdateAsync(CompletedDealEntity dealEntity)
        {
            _context.CompletedDeals.Update(dealEntity);
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

        public async Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync()
        {
            var deals = await _context.CompletedDeals.ToListAsync();
            return Result.Success<IEnumerable<CompletedDealEntity>>(deals);
        }
    }
}