using UseCases.Property.Commands.UpdateProperty;
using UseCases.UseCases.DTO.Property;

namespace Presenter.DTOs.PropertyDTO.UpdateProperty;


    public static class UpdatePropertyMapper
    {
        public static UpdatePropertyCommand ToCommand(Guid id, UpdatePropertyRequest request)
        => new(
        id,
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
