using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Property;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;
using Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreatePropertyCommand : ICommand<Property>
    {
        public Address Address { get; set; }
        public Price Price { get; set; }
        public Description Description { get; set; }
        public PropertyDetails Details { get; set; }
        public OwnershipRecord OwnerRecord { get; set; }
    }
}