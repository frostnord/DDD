using UseCases.Property.Commands.CreateProperty;
using UseCases.UseCases.DTO.Property;

namespace Presenter.DTOs.PropertyDTO.CreatePoperty;

public static class CreatePropertyMapping
{
    public static CreatePropertyCommand ToCommand(CreatePropertyRequest request)
        => new(
            new AddressDto(
                request.Address.Street,
                request.Address.City,
                request.Address.HomeNumber,
                request.Address.ZipCode,
                request.Address.Country
            ),
            new UseCases.UseCases.DTO.Property.PropertyDetailsDto(
                request.PropertyDetails.Price,
                request.PropertyDetails.Description,
                request.PropertyDetails.NumberOfRooms,
                request.PropertyDetails.Floor,
                request.PropertyDetails.TotalFloors,
                request.PropertyDetails.Area,
                request.PropertyDetails.Type,
                request.PropertyDetails.HeatingType,
                request.PropertyDetails.Condition,
                request.PropertyDetails.HasParking
            ),
            new UseCases.UseCases.DTO.Property.OwnershipDto(
                request.Ownership.OwnerClientId,
                request.Ownership.StartDate
            )
        );
}