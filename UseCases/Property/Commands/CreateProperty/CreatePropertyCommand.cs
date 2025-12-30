using UseCases.Interfaces.Commands;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Commands.CreateProperty
{
    public record CreatePropertyCommand( 
        AddressData AddressData, 
        PropertyDetailsData PropertyDetailsData,
        OwnershipData OwnershipData
        ) : ICommand<Guid>;
}