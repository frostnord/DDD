using Domain.Property;
using Domain.Property.VO;
using Domain.ValueObjects;

namespace UseCases.Clients.Commands
{
    public class CreatePropertyCommand : ICommand<PropertyEntity>
    {
        public Address Address { get; set; }
        public Price Price { get; set; }
        public Description Description { get; set; }
        public PropertyDetails Details { get; set; }
        public OwnershipRecord OwnerRecord { get; set; }
    }
}