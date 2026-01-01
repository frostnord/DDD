using UseCases.Property.Commands.CreateProperty;
using UseCases.UseCases.DTO.Property;

namespace Presenter.DTOs.PropertyDTO.CreatePoperty;

public static class CreatePropertyMapping
{
    public static CreatePropertyCommand ToCommand(CreatePropertyRequest request)
    {
        if (request?.Address == null)
            throw new ArgumentNullException(nameof(request.Address), "Address is required");
        if (request.PropertyDetails == null)
            throw new ArgumentNullException(nameof(request.PropertyDetails), "PropertyDetails is required");
        if (request.Ownership == null)
            throw new ArgumentNullException(nameof(request.Ownership), "Ownership is required");

        return new CreatePropertyCommand(
            new UseCases.UseCases.DTO.Property.AddressDto(
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
}