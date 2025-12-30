using Presenter.DTOs.PropertyDTO;
using UseCases.Property;
using UseCases.Property.Commands.CreateProperty;
using UseCases.UseCases.DTO.Property;

namespace Presenter.Mappings;

public static class CreatePropertyMapping
{
    public static CreatePropertyCommand ToCommand(CreatePropertyRequest request)
        => new(
            new AddressData(
                request.Address.Street,
                request.Address.City,
                request.Address.HomeNumber,
                request.Address.ZipCode,
                request.Address.Country
            ),
            new PropertyDetailsData(
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
            new OwnershipData(
                request.Ownership.OwnerClientId,
                request.Ownership.StartDate
            )
        );
}