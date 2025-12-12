using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
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

        public async Task<Result<PropertyEntity>> GetByIdAsync(PropertyId id)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            return property != null
                ? Result.Success(property)
                : Result.Failure<PropertyEntity>($"Property with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<PropertyEntity>>> GetAllAsync()
        {
            var properties = await _context.Properties.ToListAsync();
            return Result.Success<IEnumerable<PropertyEntity>>(properties);
        }

        public async Task<Result<PropertyEntity>> AddAsync(PropertyEntity propertyEntity)
        {
            await _context.Properties.AddAsync(propertyEntity);
            await _context.SaveChangesAsync();
            return Result.Success(propertyEntity);
        }

        public async Task<Result> UpdateAsync(PropertyEntity propertyEntity)
        {
            _context.Properties.Update(propertyEntity);
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