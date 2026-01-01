using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;
using PropertyDto = UseCases.UseCases.DTO.Property.PropertyDto;
using AddressDto = UseCases.UseCases.DTO.Property.AddressDto;
using PropertyDetailsDto = UseCases.UseCases.DTO.Property.PropertyDetailsDto;
using OwnershipDto = UseCases.UseCases.DTO.Property.OwnershipDto;

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

        public async Task<PropertyDto?> GetDtoByIdAsync(Guid id)
        {
            return await _context.Properties
                .Where(p => p.Id.Value == id)  // VO.Value → Guid
                .Select(p => new PropertyDto(
                    p.Id.Value,
                    new AddressDto(p.Address.Street, p.Address.City, p.Address.HomeNumber, p.Address.ZipCode, p.Address.Country),
                    new PropertyDetailsDto(
                        p.Price.Value,
                        p.Description.Value,
                        p.PropertyDetails.NumberOfRooms.Value,
                        p.PropertyDetails.Floor.Value,
                        p.PropertyDetails.TotalFloors.Value,
                        p.PropertyDetails.Area.Value,
                        p.PropertyDetails.Type.Name,
                        p.PropertyDetails.HeatingType.Value.ToString(),
                        p.PropertyDetails.Condition.Value,
                        p.PropertyDetails.HasParking
                    ),
                    new OwnershipDto(
                        p.OwnershipHistory.Any() ? p.OwnershipHistory.Last().OwnerClientId.Value : Guid.Empty,
                        p.OwnershipHistory.Any() ? p.OwnershipHistory.Last().StartDate : DateTime.MinValue
                    )
                ))
                .FirstOrDefaultAsync();
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