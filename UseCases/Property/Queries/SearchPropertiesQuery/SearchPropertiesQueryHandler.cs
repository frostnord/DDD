using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.Property.Queries.GetPropertyById;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.SearchPropertiesQuery
{
    public class SearchPropertiesQueryHandler : IQueryHandler<SearchPropertiesQuery, Result<SearchPropertiesQueryResponse>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public SearchPropertiesQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<SearchPropertiesQueryResponse>> HandleAsync(SearchPropertiesQuery query)
        {
            var propertiesResult = await _propertyRepository.GetAllAsync();
            if (propertiesResult.IsFailure)
                return Result.Failure<SearchPropertiesQueryResponse>(propertiesResult.Error);

            var properties = propertiesResult.Value.AsEnumerable();

            // Применяем фильтры
            if (!string.IsNullOrEmpty(query.City))
                properties = properties.Where(p => 
                    p.Address.City.Equals(query.City, System.StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(query.PropertyType))
                properties = properties.Where(p => 
                    p.PropertyDetails.Type.Name.Equals(query.PropertyType, System.StringComparison.OrdinalIgnoreCase));

            if (query.MinPrice.HasValue)
                properties = properties.Where(p => p.Price.Value >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                properties = properties.Where(p => p.Price.Value <= query.MaxPrice.Value);

            if (query.MinArea.HasValue)
                properties = properties.Where(p => p.PropertyDetails.Area.Value >= query.MinArea.Value);

            if (query.MaxArea.HasValue)
                properties = properties.Where(p => p.PropertyDetails.Area.Value <= query.MaxArea.Value);

            if (query.MinRooms.HasValue)
                properties = properties.Where(p => p.PropertyDetails.NumberOfRooms.Value >= query.MinRooms.Value);

            if (query.MaxRooms.HasValue)
                properties = properties.Where(p => p.PropertyDetails.NumberOfRooms.Value <= query.MaxRooms.Value);

            if (query.MinFloor.HasValue)
                properties = properties.Where(p => p.PropertyDetails.Floor.Value >= query.MinFloor.Value);

            if (query.MaxFloor.HasValue)
                properties = properties.Where(p => p.PropertyDetails.Floor.Value <= query.MaxFloor.Value);

            if (!string.IsNullOrEmpty(query.HeatingType))
                properties = properties.Where(p => 
                    System.Object.Equals(p.PropertyDetails.HeatingType.Value, query.HeatingType));

            if (!string.IsNullOrEmpty(query.PropertyCondition))
                properties = properties.Where(p => 
                    p.PropertyDetails.Condition.Value.Equals(query.PropertyCondition, System.StringComparison.OrdinalIgnoreCase));

            if (query.HasParking.HasValue)
                properties = properties.Where(p => p.PropertyDetails.HasParking == query.HasParking.Value);

            // Применяем сортировку
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                properties = query.SortBy.ToLower() switch
                {
                    "price" => query.SortOrder.ToLower() == "desc" 
                        ? properties.OrderByDescending(p => p.Price.Value)
                        : properties.OrderBy(p => p.Price.Value),
                    "area" => query.SortOrder.ToLower() == "desc"
                        ? properties.OrderByDescending(p => p.PropertyDetails.Area.Value)
                        : properties.OrderBy(p => p.PropertyDetails.Area.Value),
                    "rooms" => query.SortOrder.ToLower() == "desc"
                        ? properties.OrderByDescending(p => p.PropertyDetails.NumberOfRooms.Value)
                        : properties.OrderBy(p => p.PropertyDetails.NumberOfRooms.Value),
                    "city" => query.SortOrder.ToLower() == "desc"
                        ? properties.OrderByDescending(p => p.Address.City)
                        : properties.OrderBy(p => p.Address.City),
                    _ => properties
                };
            }
            else
            {
                properties = properties.OrderBy(p => p.CreatedAt);
            }

            var allProperties = properties.ToList();
            var totalCount = allProperties.Count;

            // Применяем пагинацию
            var pagedProperties = allProperties
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var items = pagedProperties.Select(p => new PropertyDto(
                p.Id.Value,
                new AddressDto(
                    p.Address.Street,
                    p.Address.City,
                    p.Address.HomeNumber,
                    p.Address.ZipCode,
                    p.Address.Country
                ),
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
                    p.GetCurrentOwner()?.OwnerClientId?.Value ?? Guid.Empty,
                    p.GetCurrentOwner()?.StartDate ?? DateTime.MinValue
                )
            )).ToList();

            var response = new SearchPropertiesQueryResponse(
                items,
                totalCount,
                query.PageSize,
                (int)System.Math.Ceiling((double)totalCount / query.PageSize)
            );

            return Result.Success(response);
        }
    }
}