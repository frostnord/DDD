using CSharpFunctionalExtensions;
using Domain.Domain.Property;
using Domain.Domain.Property.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _context;

        public PropertyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Property>> GetByIdAsync(PropertyId id)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            return property != null
                ? Result.Success(property)
                : Result.Failure<Property>($"Property with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Property>>> GetAllAsync()
        {
            var properties = await _context.Properties.ToListAsync();
            return Result.Success<IEnumerable<Property>>(properties);
        }

        public async Task<Result<Property>> AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
            return Result.Success(property);
        }

        public async Task<Result> UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(PropertyId id)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return Result.Failure($"Property with ID {id.Value} not found");
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(PropertyId id)
        {
            return await _context.Properties.AnyAsync(p => p.Id == id);
        }
    }
}