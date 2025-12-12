using CSharpFunctionalExtensions;
using Domain.Agency;
using Domain.Agency.VO;
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

        public async Task<Result<AgencyEntity>> GetByIdAsync(AgencyId id)
        {
            var agency = await _context.Agencies
                .FirstOrDefaultAsync(a => a.Id == id);

            return agency != null
                ? Result.Success(agency)
                : Result.Failure<AgencyEntity>($"Agency with ID {id.Value} not found");
        }

        public async Task<Result> SaveAsync(AgencyEntity agencyEntity)
        {
            if (agencyEntity == null)
                return Result.Failure("Agency cannot be null");

            _context.Agencies.Update(agencyEntity);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(AgencyEntity agencyEntity)
        {
            if (agencyEntity == null)
                return Result.Failure("Agency cannot be null");

            _context.Agencies.Update(agencyEntity);
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