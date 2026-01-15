using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.GetPropertyById;

public class GetPropertyByIdQueryHandler : IQueryHandler<GetPropertyByIdQuery, Result<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<PropertyDto>> HandleAsync(GetPropertyByIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.PropertyId);
        if (propertyIdResult.IsFailure)
            return Result.Failure<PropertyDto>(propertyIdResult.Error);

        var propertyResult = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
        if (propertyResult.IsFailure)
            return Result.Failure<PropertyDto>(propertyResult.Error);

        var entity = propertyResult.Value;
        var currentOwner = entity.OwnershipHistory.LastOrDefault();

        // Маппинг из Entity в DTO происходит здесь, в слое Use Case
        var propertyDto = new PropertyDto(
            entity.Id.Value,
            new AddressDto(
                entity.Address.Street,
                entity.Address.City,
                entity.Address.HomeNumber,
                entity.Address.ZipCode,
                entity.Address.Country),
            new PropertyDetailsDto(
                entity.Price.Value,
                entity.Description.Value,
                entity.PropertyDetails.NumberOfRooms.Value,
                entity.PropertyDetails.Floor.Value,
                entity.PropertyDetails.TotalFloors.Value,
                entity.PropertyDetails.Area.Value,
                entity.PropertyDetails.Type.Name,
                entity.PropertyDetails.HeatingType.Value.ToString(),
                entity.PropertyDetails.Condition.Value,
                entity.PropertyDetails.HasParking),
            new OwnershipDto(currentOwner?.OwnerClientId.Value ?? Guid.Empty, currentOwner?.StartDate ?? DateTime.MinValue));
        return Result.Success(propertyDto);
    }
}