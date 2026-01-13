using UseCases.Interfaces.Commands;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Commands.UpdateProperty
{
    public record UpdatePropertyCommand(
        Guid PropertyId,
        AddressDto AddressDto,
        PropertyDetailsDto PropertyDetailsDto,
        OwnershipDto OwnershipDto
    ) : ICommand;

}