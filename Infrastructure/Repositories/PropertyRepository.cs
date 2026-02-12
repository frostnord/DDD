using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property;
using Domain.Property.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;
using UseCases.Property.Queries.SearchPropertiesQuery;
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
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return property != null
                ? Result.Success(property)
                : Result.Failure<PropertyEntity>($"Property with ID {id.Value} not found");
        }

        public async Task<Result<PropertyEntity>> GetByIdForUpdateAsync(PropertyId id)
        {
            var property = await _context.Properties
                .FromSqlInterpolated($@"SELECT * FROM property WHERE id = {id.Value} FOR UPDATE")
                .FirstOrDefaultAsync(p => p.Id == id);

            return property != null
                ? Result.Success(property)
                : Result.Failure<PropertyEntity>($"Property with ID {id.Value} not found");
        }


        public async Task<Result<IEnumerable<PropertyEntity>>> GetAllAsync()
        {
            var properties = await _context.Properties.AsNoTracking().ToListAsync();
            return Result.Success<IEnumerable<PropertyEntity>>(properties);
        }

        public async Task<Result<(IEnumerable<PropertyEntity> Items, int TotalCount)>> SearchAsync(
            SearchPropertiesQuery query)
        {
            IQueryable<PropertyEntity> properties = _context.Properties.AsNoTracking();

            if (!string.IsNullOrEmpty(query.City))
            {
                properties = properties.Where(p =>
                    p.Address.City.Equals(query.City, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(query.PropertyType))
            {
                properties = properties.Where(p =>
                    p.PropertyDetails.Type.Name.Equals(query.PropertyType, System.StringComparison.OrdinalIgnoreCase));
            }

            if (query.MinPrice.HasValue)
            {
                properties = properties.Where(p => p.Price.Value >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                properties = properties.Where(p => p.Price.Value <= query.MaxPrice.Value);
            }

            if (query.MinArea.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.Area.Value >= query.MinArea.Value);
            }

            if (query.MaxArea.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.Area.Value <= query.MaxArea.Value);
            }

            if (query.MinRooms.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.NumberOfRooms.Value >= query.MinRooms.Value);
            }

            if (query.MaxRooms.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.NumberOfRooms.Value <= query.MaxRooms.Value);
            }

            if (query.MinFloor.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.Floor.Value >= query.MinFloor.Value);
            }

            if (query.MaxFloor.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.Floor.Value <= query.MaxFloor.Value);
            }

            if (!string.IsNullOrEmpty(query.HeatingType))
            {
                properties = properties.Where(p =>
                    System.Object.Equals(p.PropertyDetails.HeatingType.Value, query.HeatingType));
            }

            if (!string.IsNullOrEmpty(query.PropertyCondition))
            {
                properties = properties.Where(p =>
                    p.PropertyDetails.Condition.Value.Equals(query.PropertyCondition, System.StringComparison.OrdinalIgnoreCase));
            }

            if (query.HasParking.HasValue)
            {
                properties = properties.Where(p => p.PropertyDetails.HasParking == query.HasParking.Value);
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var sortBy = query.SortBy.ToLower();
                var sortOrder = query.SortOrder?.ToLower();

                properties = sortBy switch
                {
                    "price" => sortOrder == "desc"
                        ? properties.OrderByDescending(p => p.Price.Value)
                        : properties.OrderBy(p => p.Price.Value),
                    "area" => sortOrder == "desc"
                        ? properties.OrderByDescending(p => p.PropertyDetails.Area.Value)
                        : properties.OrderBy(p => p.PropertyDetails.Area.Value),
                    "rooms" => sortOrder == "desc"
                        ? properties.OrderByDescending(p => p.PropertyDetails.NumberOfRooms.Value)
                        : properties.OrderBy(p => p.PropertyDetails.NumberOfRooms.Value),
                    "city" => sortOrder == "desc"
                        ? properties.OrderByDescending(p => p.Address.City)
                        : properties.OrderBy(p => p.Address.City),
                    _ => properties
                };
            }
            else
            {
                properties = properties.OrderBy(p => p.CreatedAt);
            }

            var totalCount = await properties.CountAsync();
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 1 : query.PageSize;

            var items = await properties
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result.Success((items.AsEnumerable(), totalCount));
        }

        public async Task<Result<PropertyEntity?>> GetActiveReservationByPropertyIdAsync(PropertyId propertyId, DateTime nowUtc)
        {
            var property = await _context.Properties
                .AsNoTracking()
                .Where(p => p.Id == propertyId
                            && p.ReservedUntil != null
                            && p.ReservedUntil > nowUtc)
                .FirstOrDefaultAsync();

            return Result.Success(property);
        }

        public async Task<Result<IEnumerable<PropertyEntity>>> GetActiveReservationByClientIdAsync(ClientId clientId, DateTime nowUtc)
        {
            var properties = await _context.Properties
                .AsNoTracking()
                .Where(p => p.ReservedByClientId == clientId
                            && p.ReservedUntil != null
                            && p.ReservedUntil > nowUtc)
                .ToListAsync();

            return Result.Success<IEnumerable<PropertyEntity>>(properties);
        }

        public Result<PropertyEntity> Add(PropertyEntity propertyEntity)
        {
            _context.Properties.Add(propertyEntity);
            return Result.Success(propertyEntity);
        }

        public Result Update(PropertyEntity propertyEntity)
        {
            _context.Properties.Update(propertyEntity);
            return Result.Success();
        }

        public Result Delete(PropertyId id)
        {
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);
            if (property == null)
            {
                return Result.Failure($"Property with ID {id.Value} not found");
            }

            _context.Properties.Remove(property);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(PropertyId id)
        {
            return await _context.Properties.AsNoTracking().AnyAsync(p => p.Id == id);
        }
    }
}
