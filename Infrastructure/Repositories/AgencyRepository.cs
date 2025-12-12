using CSharpFunctionalExtensions;
using Domain.Domain.Agency;
using Domain.Domain.Agency.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class AgencyRepository : IAgencyRepository
    {
        private readonly AppDbContext _context;

        public AgencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Agency>> GetByIdAsync(AgencyId id)
        {
            var agency = await _context.Agencies
                .FirstOrDefaultAsync(a => a.Id == id);

            return agency != null
                ? Result.Success(agency)
                : Result.Failure<Agency>($"Agency with ID {id.Value} not found");
        }

        public async Task<Result> SaveAsync(Agency agency)
        {
            if (agency == null)
                return Result.Failure("Agency cannot be null");

            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(Agency agency)
        {
            if (agency == null)
                return Result.Failure("Agency cannot be null");

            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(AgencyId id)
        {
            var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.Id == id);
            if (agency == null)
            {
                return Result.Failure($"Agency with ID {id.Value} not found");
            }

            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}