using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;

namespace UseCases.Property
{
    public record CreatePropertyCommand(
        Address Address,
        Price Price,
        Description Description,
        PropertyDetails Details,
        OwnershipRecord OwnerRecord
    ) : ICommand<PropertyEntity>;
}