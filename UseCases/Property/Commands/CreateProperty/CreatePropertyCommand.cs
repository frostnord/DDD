using UseCases.Interfaces.Commands;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Commands.CreateProperty;

public record CreatePropertyCommand( 
    AddressDto AddressDto, 
    PropertyDetailsDto PropertyDetailsDto,
    OwnershipDto OwnershipDto
    ) : ICommand<Guid>;